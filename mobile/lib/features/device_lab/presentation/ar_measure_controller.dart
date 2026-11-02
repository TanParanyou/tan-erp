import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/features/device_lab/data/ar_measure_repository.dart';
import 'package:tan_erp_field/features/device_lab/domain/ar_measure_failure.dart';

final arAvailabilityProvider = FutureProvider<ArAvailability>(
  (ref) => ref.watch(arMeasureRepositoryProvider).checkAvailability(),
);

/// Unknown may still work: ARCore's install flow gives the final answer.
/// Null (still checking) counts as not startable.
bool arCanStart(ArAvailability? availability) => switch (availability) {
  ArAvailability.supported ||
  ArAvailability.needsInstall ||
  ArAvailability.unknown => true,
  ArAvailability.unsupported || null => false,
};

@immutable
class ArMeasureState {
  const ArMeasureState({this.isMeasuring = false, this.result, this.failure});

  final bool isMeasuring;
  final ArMeasureResult? result;
  final ArMeasureFailure? failure;
}

/// Runs one native measurement; keeps the last result when the user cancels.
class ArMeasureController extends Notifier<ArMeasureState> {
  @override
  ArMeasureState build() => const ArMeasureState();

  /// Returns this attempt's result only: null when cancelled, failed (see
  /// [ArMeasureState.failure]) or already measuring. [ArMeasureState.result]
  /// still keeps the previous result for display.
  Future<ArMeasureResult?> measure(ArMeasureRequest request) async {
    if (state.isMeasuring) return null;
    final previous = state.result;
    state = ArMeasureState(isMeasuring: true, result: previous);
    ArMeasureResult? result;
    try {
      result = await ref
          .read(arMeasureRepositoryProvider)
          .measureFloor(request);
      state = ArMeasureState(result: result ?? previous);
    } on ArMeasureFailure catch (e) {
      state = ArMeasureState(result: previous, failure: e);
    }
    // ARCore may have been installed or updated during the attempt.
    ref.invalidate(arAvailabilityProvider);
    return result;
  }
}

final arMeasureControllerProvider =
    NotifierProvider<ArMeasureController, ArMeasureState>(
      ArMeasureController.new,
    );
