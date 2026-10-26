//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/defect_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/handover_response.dart';
import 'package:tan_erp_api/src/model/warranty_ref_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/checklist_item_response.dart';
import 'package:tan_erp_api/src/model/service_person_response.dart';
import 'package:tan_erp_api/src/model/service_project_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'installation_response.g.dart';

/// InstallationResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [status] 
/// * [project] 
/// * [scheduledStart] 
/// * [scheduledEnd] 
/// * [crewName] 
/// * [note] 
/// * [cancelReason] 
/// * [readyAtUtc] 
/// * [handover] 
/// * [disputeCount] 
/// * [lastDisputeNote] 
/// * [warranty] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [rowVersion] 
/// * [checklist] 
/// * [defects] 
@BuiltValue()
abstract class InstallationResponse implements Built<InstallationResponse, InstallationResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'project')
  ServiceProjectRefResponse? get project;

  @BuiltValueField(wireName: r'scheduledStart')
  Date? get scheduledStart;

  @BuiltValueField(wireName: r'scheduledEnd')
  Date? get scheduledEnd;

  @BuiltValueField(wireName: r'crewName')
  String? get crewName;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'cancelReason')
  String? get cancelReason;

  @BuiltValueField(wireName: r'readyAtUtc')
  DateTime? get readyAtUtc;

  @BuiltValueField(wireName: r'handover')
  HandoverResponse? get handover;

  @BuiltValueField(wireName: r'disputeCount')
  int? get disputeCount;

  @BuiltValueField(wireName: r'lastDisputeNote')
  String? get lastDisputeNote;

  @BuiltValueField(wireName: r'warranty')
  WarrantyRefResponse? get warranty;

  @BuiltValueField(wireName: r'createdBy')
  ServicePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'checklist')
  BuiltList<ChecklistItemResponse>? get checklist;

  @BuiltValueField(wireName: r'defects')
  BuiltList<DefectResponse>? get defects;

  InstallationResponse._();

  factory InstallationResponse([void updates(InstallationResponseBuilder b)]) = _$InstallationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InstallationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InstallationResponse> get serializer => _$InstallationResponseSerializer();
}

class _$InstallationResponseSerializer implements PrimitiveSerializer<InstallationResponse> {
  @override
  final Iterable<Type> types = const [InstallationResponse, _$InstallationResponse];

  @override
  final String wireName = r'InstallationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InstallationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(ServiceProjectRefResponse),
      );
    }
    if (object.scheduledStart != null) {
      yield r'scheduledStart';
      yield serializers.serialize(
        object.scheduledStart,
        specifiedType: const FullType(Date),
      );
    }
    if (object.scheduledEnd != null) {
      yield r'scheduledEnd';
      yield serializers.serialize(
        object.scheduledEnd,
        specifiedType: const FullType(Date),
      );
    }
    if (object.crewName != null) {
      yield r'crewName';
      yield serializers.serialize(
        object.crewName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.cancelReason != null) {
      yield r'cancelReason';
      yield serializers.serialize(
        object.cancelReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.readyAtUtc != null) {
      yield r'readyAtUtc';
      yield serializers.serialize(
        object.readyAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.handover != null) {
      yield r'handover';
      yield serializers.serialize(
        object.handover,
        specifiedType: const FullType(HandoverResponse),
      );
    }
    if (object.disputeCount != null) {
      yield r'disputeCount';
      yield serializers.serialize(
        object.disputeCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.lastDisputeNote != null) {
      yield r'lastDisputeNote';
      yield serializers.serialize(
        object.lastDisputeNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.warranty != null) {
      yield r'warranty';
      yield serializers.serialize(
        object.warranty,
        specifiedType: const FullType(WarrantyRefResponse),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ServicePersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.checklist != null) {
      yield r'checklist';
      yield serializers.serialize(
        object.checklist,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ChecklistItemResponse)]),
      );
    }
    if (object.defects != null) {
      yield r'defects';
      yield serializers.serialize(
        object.defects,
        specifiedType: const FullType.nullable(BuiltList, [FullType(DefectResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    InstallationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InstallationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServiceProjectRefResponse),
          ) as ServiceProjectRefResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'scheduledStart':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.scheduledStart = valueDes;
          break;
        case r'scheduledEnd':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.scheduledEnd = valueDes;
          break;
        case r'crewName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.crewName = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'cancelReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.cancelReason = valueDes;
          break;
        case r'readyAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.readyAtUtc = valueDes;
          break;
        case r'handover':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(HandoverResponse),
          ) as HandoverResponse?;
          if (valueDes == null) continue;
          result.handover.replace(valueDes);
          break;
        case r'disputeCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.disputeCount = valueDes;
          break;
        case r'lastDisputeNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lastDisputeNote = valueDes;
          break;
        case r'warranty':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WarrantyRefResponse),
          ) as WarrantyRefResponse?;
          if (valueDes == null) continue;
          result.warranty.replace(valueDes);
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'checklist':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ChecklistItemResponse)]),
          ) as BuiltList<ChecklistItemResponse>?;
          if (valueDes == null) continue;
          result.checklist.replace(valueDes);
          break;
        case r'defects':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(DefectResponse)]),
          ) as BuiltList<DefectResponse>?;
          if (valueDes == null) continue;
          result.defects.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InstallationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InstallationResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


