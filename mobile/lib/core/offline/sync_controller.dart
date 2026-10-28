import 'dart:async';
import 'dart:convert';
import 'dart:io';

import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/offline/offline_store.dart';
import 'package:uuid/uuid.dart';

@immutable
class SyncState {
  const SyncState({
    this.isOnline = true,
    this.isSyncing = false,
    this.mutations = const [],
  });

  final bool isOnline;
  final bool isSyncing;
  final List<OutboxMutation> mutations;

  int get pendingCount => mutations
      .where(
        (m) =>
            m.status == OutboxStatus.pending ||
            m.status == OutboxStatus.syncing,
      )
      .length;

  int get conflictCount =>
      mutations.where((m) => m.status == OutboxStatus.conflict).length;

  int get failedCount =>
      mutations.where((m) => m.status == OutboxStatus.failed).length;

  bool get shouldShowBanner =>
      !isOnline || isSyncing || mutations.isNotEmpty;

  SyncState copyWith({
    bool? isOnline,
    bool? isSyncing,
    List<OutboxMutation>? mutations,
  }) => SyncState(
    isOnline: isOnline ?? this.isOnline,
    isSyncing: isSyncing ?? this.isSyncing,
    mutations: mutations ?? this.mutations,
  );
}

class SyncController extends Notifier<SyncState> {
  static const _uuid = Uuid();
  StreamSubscription<List<ConnectivityResult>>? _connectivitySub;

  @override
  SyncState build() {
    final membershipId = ref.watch(activeMembershipIdProvider) ?? 'default';
    unawaited(_init(membershipId));
    ref.onDispose(() => _connectivitySub?.cancel());
    return const SyncState();
  }

  Future<void> _init(String membershipId) async {
    final store = ref.read(offlineStoreProvider);
    final list = await store.listMutations(membershipId);
    var online = true;
    try {
      final results = await Connectivity().checkConnectivity();
      online = !results.contains(ConnectivityResult.none);
    } on Object {
      online = true;
    }
    state = state.copyWith(isOnline: online, mutations: list);

    await _connectivitySub?.cancel();
    _connectivitySub = Connectivity().onConnectivityChanged.listen((results) {
      final nowOnline = !results.contains(ConnectivityResult.none);
      final wasOffline = !state.isOnline;
      state = state.copyWith(isOnline: nowOnline);
      if (nowOnline && wasOffline) {
        unawaited(flushNow());
      }
    });

    if (online && list.any((m) => m.status == OutboxStatus.pending)) {
      unawaited(flushNow());
    }
  }

  Future<void> reload() async {
    final membershipId = ref.read(activeMembershipIdProvider) ?? 'default';
    final list = await ref.read(offlineStoreProvider).listMutations(
      membershipId,
    );
    state = state.copyWith(mutations: list);
  }

  Future<OutboxMutation> enqueue({
    required String entityType,
    required String entityId,
    required String operation,
    required String title,
    required Map<String, Object?> payload,
    String? expectedRowVersion,
  }) async {
    final membershipId = ref.read(activeMembershipIdProvider) ?? 'default';
    final store = ref.read(offlineStoreProvider);
    final item = OutboxMutation(
      id: _uuid.v4(),
      membershipId: membershipId,
      entityType: entityType,
      entityId: entityId,
      operation: operation,
      title: title,
      payloadJson: jsonEncode(payload),
      idempotencyKey: _uuid.v4(),
      expectedRowVersion: expectedRowVersion,
      status: OutboxStatus.pending,
      createdAtUtc: DateTime.now().toUtc(),
    );
    await store.enqueueMutation(item);
    await reload();
    if (state.isOnline) {
      unawaited(flushNow());
    }
    return item;
  }

  Future<void> flushNow() async {
    if (state.isSyncing) return;
    final membershipId = ref.read(activeMembershipIdProvider) ?? 'default';
    final store = ref.read(offlineStoreProvider);
    final current = await store.listMutations(membershipId);
    final pending = current
        .where(
          (m) =>
              m.status == OutboxStatus.pending ||
              m.status == OutboxStatus.failed,
        )
        .toList();
    if (pending.isEmpty) {
      state = state.copyWith(mutations: current);
      return;
    }

    state = state.copyWith(isSyncing: true, mutations: current);
    for (final m in pending) {
      await store.updateMutationStatus(id: m.id, status: OutboxStatus.syncing);
      await reload();
      try {
        await _dispatchMutation(m);
        await store.deleteMutation(m.id);
      } on ConflictFailure catch (f) {
        await store.updateMutationStatus(
          id: m.id,
          status: OutboxStatus.conflict,
          errorCode: f.code,
        );
      } on NetworkFailure {
        await store.updateMutationStatus(
          id: m.id,
          status: OutboxStatus.pending,
        );
        state = state.copyWith(isOnline: false, isSyncing: false);
        await reload();
        return;
      } on ApiFailure catch (f) {
        await store.updateMutationStatus(
          id: m.id,
          status: OutboxStatus.failed,
          errorCode: f.code,
        );
      } on Object {
        await store.updateMutationStatus(
          id: m.id,
          status: OutboxStatus.failed,
          errorCode: 'SYNC_FAILED',
        );
      }
      await reload();
    }
    state = state.copyWith(isSyncing: false);
    await reload();
  }

  Future<void> clearFailedAndConflicts() async {
    final membershipId = ref.read(activeMembershipIdProvider) ?? 'default';
    await ref.read(offlineStoreProvider).clearNonPendingMutations(membershipId);
    await reload();
  }

  Future<void> removeMutation(String id) async {
    await ref.read(offlineStoreProvider).deleteMutation(id);
    await reload();
  }

  Future<void> retryMutation(String id) async {
    await ref
        .read(offlineStoreProvider)
        .updateMutationStatus(id: id, status: OutboxStatus.pending);
    await reload();
    if (state.isOnline) {
      unawaited(flushNow());
    }
  }

  Future<void> _dispatchMutation(OutboxMutation m) async {
    final client = ref.read(apiClientProvider);
    final decoded = jsonDecode(m.payloadJson);
    final data = decoded is Map<String, Object?>
        ? decoded
        : <String, Object?>{};

    switch ('${m.entityType}.${m.operation}') {
      case 'notification.mark_read':
        await client.call(
          () => client.api.getNotificationsApi().apiV1NotificationsIdReadPost(
            id: m.entityId,
          ),
        );
      case 'notification.mark_all_read':
        await client.call(
          () =>
              client.api.getNotificationsApi().apiV1NotificationsReadAllPost(),
        );
      case 'survey.update_draft':
        final opportunityId = data['opportunityId'] as String? ?? '';
        final surveyId = data['surveyId'] as String? ?? '';
        final revisionId = data['revisionId'] as String? ?? m.entityId;
        final scopeSummary = data['scopeSummary'] as String? ?? '';
        final areasRaw = data['areas'];
        final areaBuilders = <UpdateSurveyAreaRequest>[];
        if (areasRaw is List<Object?>) {
          for (var i = 0; i < areasRaw.length; i++) {
            final item = areasRaw[i];
            if (item is! Map<String, Object?>) continue;
            final measRaw = item['measurements'];
            final measList = <UpdateSurveyMeasurementRequest>[];
            if (measRaw is List<Object?>) {
              for (var j = 0; j < measRaw.length; j++) {
                final mr = measRaw[j];
                if (mr is! Map<String, Object?>) continue;
                final valNum = mr['value'];
                final valDouble = valNum is num ? valNum.toDouble() : 0.0;
                measList.add(
                  UpdateSurveyMeasurementRequest(
                    (b) => b
                      ..id = mr['id'] as String?
                      ..measurementType =
                          mr['measurementType'] as String? ?? 'area'
                      ..value = valDouble
                      ..unitCode = mr['unitCode'] as String? ?? 'sqm'
                      ..captureMethod =
                          mr['captureMethod'] as String? ?? 'measured'
                      ..notes = mr['notes'] as String?
                      ..sortOrder = j + 1,
                  ),
                );
              }
            }
            areaBuilders.add(
              UpdateSurveyAreaRequest(
                (b) => b
                  ..id = item['id'] as String?
                  ..code = item['code'] as String? ?? 'A-${i + 1}'
                  ..name = item['name'] as String? ?? ''
                  ..description = item['description'] as String?
                  ..sortOrder = i + 1
                  ..measurements.replace(measList),
              ),
            );
          }
        }
        final req = UpdateSurveyDraftRequest(
          (b) => b
            ..expectedRevisionVersion = m.expectedRowVersion ?? '1'
            ..visitedAtUtc = DateTime.now().toUtc()
            ..scopeSummary = scopeSummary
            ..areas.replace(areaBuilders),
        );
        final surveysApi = client.api.getSiteSurveysApi();
        await client.call(
          () => surveysApi
              // Generated method name exceeds 80 chars.
              // ignore: lines_longer_than_80_chars
              .apiV1OpportunitiesOpportunityIdSurveysSurveyIdRevisionsRevisionIdDraftPut(
                opportunityId: opportunityId,
                surveyId: surveyId,
                revisionId: revisionId,
                updateSurveyDraftRequest: req,
              ),
        );
      case 'survey.mark_ready':
        final opportunityId = data['opportunityId'] as String? ?? '';
        final surveyId = data['surveyId'] as String? ?? '';
        final revisionId = data['revisionId'] as String? ?? m.entityId;
        final req = MarkSurveyReadyRequest(
          (b) => b..expectedRevisionVersion = m.expectedRowVersion ?? '1',
        );
        final surveysApi = client.api.getSiteSurveysApi();
        await client.call(
          () => surveysApi
              // Generated method name exceeds 80 chars.
              // ignore: lines_longer_than_80_chars
              .apiV1OpportunitiesOpportunityIdSurveysSurveyIdRevisionsRevisionIdMarkReadyPost(
                opportunityId: opportunityId,
                surveyId: surveyId,
                revisionId: revisionId,
                markSurveyReadyRequest: req,
              ),
        );
      case 'attachment.upload_and_link':
        final ownerType = data['ownerType'] as String? ?? 'survey-area';
        final ownerId = data['ownerId'] as String? ?? m.entityId;
        final filename = data['filename'] as String? ?? 'photo.jpg';
        final localPath = data['localPath'] as String? ?? '';
        final purpose = data['purpose'] as String? ?? 'evidence';
        if (localPath.isEmpty || !File(localPath).existsSync()) {
          return;
        }
        final bytes = await File(localPath).readAsBytes();
        final slotReq = FileSlotRequest(
          (b) => b
            ..filename = filename
            ..mediaType = 'image/jpeg'
            ..fileSizeBytes = bytes.length,
        );
        final createReq = CreateUploadSessionRequest(
          (b) => b
            ..parentType = ownerType
            ..parentId = ownerId
            ..files.replace([slotReq]),
        );
        final filesApi = client.api.getFilesApi();
        final session = await client.call(
          () => filesApi.apiV1FilesUploadSessionsPost(
            createUploadSessionRequest: createReq,
          ),
        );
        final sessionId = session.sessionId;
        final firstSlot = session.slots?.firstOrNull;
        final slotId = firstSlot?.slotId;
        if (sessionId == null || slotId == null) return;

        final formData = FormData.fromMap(<String, Object?>{
          'file': MultipartFile.fromBytes(bytes, filename: filename),
        });
        await client.api.dio.put<Object>(
          '/api/v1/files/upload-sessions/$sessionId/slots/$slotId',
          data: formData,
        );

        final completed = await client.call(
          () => filesApi.apiV1FilesUploadSessionsSessionIdCompletePost(
            sessionId: sessionId,
          ),
        );
        final uploadedFileId = completed.files?.firstOrNull?.fileId;
        if (uploadedFileId == null) return;

        final attachReq = AttachFilesRequest(
          (b) => b
            ..purpose = purpose
            ..fileIds.replace([uploadedFileId]),
        );
        final attachmentsApi = client.api.getAttachmentsApi();
        await client.call(
          () => attachmentsApi
              .apiV1AttachmentOwnersOwnerTypeOwnerIdAttachmentsPost(
                ownerType: ownerType,
                ownerId: ownerId,
                idempotencyKey: m.idempotencyKey,
                attachFilesRequest: attachReq,
              ),
        );
      case 'estimate.review_decision':
        final revisionNoNum = data['revisionNo'];
        final revisionNo = revisionNoNum is num ? revisionNoNum.toInt() : 1;
        final decision = data['decision'] as String? ?? 'approved';
        final reasonCode = data['reasonCode'] as String?;
        final note = data['note'] as String?;
        final req = ReviewEstimateRequest(
          (b) => b
            ..revisionNo = revisionNo
            ..decision = decision
            ..reasonCode = reasonCode
            ..note = note,
        );
        await client.call(
          () => client.api
              .getEstimatesApi()
              .apiV1EstimatesIdReviewDecisionsPost(
                id: m.entityId,
                reviewEstimateRequest: req,
              ),
        );
      case 'purchase_order.approve':
        final note = data['note'] as String?;
        final req = PurchaseOrderActionRequest((b) => b..note = note);
        await client.call(
          () => client.api
              .getPurchaseOrdersApi()
              .apiV1PurchaseOrdersIdApprovePost(
                id: m.entityId,
                purchaseOrderActionRequest: req,
              ),
        );
      case 'purchase_order.reject':
        final note = data['note'] as String?;
        final req = PurchaseOrderActionRequest((b) => b..note = note);
        await client.call(
          () => client.api
              .getPurchaseOrdersApi()
              .apiV1PurchaseOrdersIdRejectPost(
                id: m.entityId,
                purchaseOrderActionRequest: req,
              ),
        );
      case 'change_order.approve':
        final projectId = data['projectId'] as String? ?? '';
        final note = data['note'] as String?;
        final req = ProjectChangeOrderActionRequest(
          (b) => b
            ..expectedVersion = m.expectedRowVersion ?? '1'
            ..note = note,
        );
        final controlApi = client.api.getProjectControlApi();
        await client.call(
          () => controlApi
              .apiV1ProjectsProjectIdChangeOrdersChangeOrderIdApprovePost(
                projectId: projectId,
                changeOrderId: m.entityId,
                projectChangeOrderActionRequest: req,
              ),
        );
      case 'change_order.reject':
        final projectId = data['projectId'] as String? ?? '';
        final note = data['note'] as String?;
        final req = ProjectChangeOrderActionRequest(
          (b) => b
              ..expectedVersion = m.expectedRowVersion ?? '1'
              ..note = note,
        );
        final controlApi = client.api.getProjectControlApi();
        await client.call(
          () => controlApi
              .apiV1ProjectsProjectIdChangeOrdersChangeOrderIdRejectPost(
                projectId: projectId,
                changeOrderId: m.entityId,
                projectChangeOrderActionRequest: req,
              ),
        );
      case 'quick_estimate.share':
        final versionNum = data['sourceVersion'];
        final sourceVersion = versionNum is num ? versionNum.toInt() : 1;
        final locale = data['locale'] as String? ?? 'th';
        final req = ShareRequest(
          (b) => b
            ..sourceVersion = sourceVersion
            ..channel = 'on_screen'
            ..locale = locale,
        );
        final qeApi = client.api.getQuickEstimatesApi();
        await client.call(
          () => qeApi.apiV1QuickEstimatesIdSharesPost(
            id: m.entityId,
            shareRequest: req,
          ),
        );
      case 'quick_estimate.convert':
        final versionNum = data['sourceVersion'];
        final sourceVersion = versionNum is num ? versionNum.toInt() : 1;
        final revId = data['siteSurveyRevisionId'] as String? ?? '';
        final req = ConversionRequest(
          (b) => b
            ..sourceVersion = sourceVersion
            ..siteSurveyRevisionId = revId,
        );
        await client.call(
          () => client.api
              .getQuickEstimatesApi()
              .apiV1QuickEstimatesIdConversionPost(
                id: m.entityId,
                conversionRequest: req,
              ),
        );
    }
  }
}

final syncControllerProvider = NotifierProvider<SyncController, SyncState>(
  SyncController.new,
);
