import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/features/scan/data/scan_repository.dart';
import 'package:tan_erp_field/features/scan/domain/scan_models.dart';

@immutable
class ScanState {
  const ScanState({
    this.isLoading = false,
    this.cameraOpen = false,
    this.currentResult,
    this.recentScans = const <ScannedItemResult>[],
    this.notFound = false,
    this.error,
  });

  final bool isLoading;
  final bool cameraOpen;
  final ScannedItemResult? currentResult;
  final List<ScannedItemResult> recentScans;
  final bool notFound;
  final ApiFailure? error;

  ScanState copyWith({
    bool? isLoading,
    bool? cameraOpen,
    ScannedItemResult? Function()? currentResult,
    List<ScannedItemResult>? recentScans,
    bool? notFound,
    ApiFailure? Function()? error,
  }) {
    return ScanState(
      isLoading: isLoading ?? this.isLoading,
      cameraOpen: cameraOpen ?? this.cameraOpen,
      currentResult:
          currentResult != null ? currentResult() : this.currentResult,
      recentScans: recentScans ?? this.recentScans,
      notFound: notFound ?? this.notFound,
      error: error != null ? error() : this.error,
    );
  }
}

class ScanController extends Notifier<ScanState> {
  @override
  ScanState build() {
    unawaited(_loadRecents());
    return const ScanState();
  }

  Future<void> _loadRecents() async {
    final repo = ref.read(scanRepositoryProvider);
    final recents = await repo.getRecentScans();
    state = state.copyWith(recentScans: recents);
  }

  void toggleCamera() {
    state = state.copyWith(cameraOpen: !state.cameraOpen);
  }

  void openCamera() {
    state = state.copyWith(cameraOpen: true);
  }

  void closeCamera() {
    state = state.copyWith(cameraOpen: false);
  }

  void selectRecent(ScannedItemResult item) {
    state = state.copyWith(
      currentResult: () => item,
      notFound: false,
      error: () => null,
      cameraOpen: false,
    );
  }

  void clearResult() {
    state = state.copyWith(
      currentResult: () => null,
      notFound: false,
      error: () => null,
    );
  }

  Future<void> lookup(String query) async {
    if (state.isLoading) return;
    state = state.copyWith(
      isLoading: true,
      notFound: false,
      error: () => null,
      currentResult: () => null,
    );

    try {
      final repo = ref.read(scanRepositoryProvider);
      final result = await repo.lookup(query);
      final recents = await repo.getRecentScans();
      state = state.copyWith(
        isLoading: false,
        currentResult: () => result,
        recentScans: recents,
        notFound: false,
        error: () => null,
      );
    } on NotFoundFailure {
      state = state.copyWith(
        isLoading: false,
        notFound: true,
        currentResult: () => null,
      );
    } on ApiFailure catch (e) {
      state = state.copyWith(
        isLoading: false,
        error: () => e,
        currentResult: () => null,
      );
    } on Object {
      state = state.copyWith(
        isLoading: false,
        error: () => const UnknownFailure(),
        currentResult: () => null,
      );
    }
  }
}

final scanControllerProvider =
    NotifierProvider<ScanController, ScanState>(ScanController.new);
