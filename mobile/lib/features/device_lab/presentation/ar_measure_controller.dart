import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/features/device_lab/data/ar_measure_repository.dart';
import 'package:tan_erp_field/features/device_lab/domain/ar_measure_failure.dart';

final arAvailabilityProvider = FutureProvider<ArAvailability>(
  (ref) => ref.watch(arMeasureRepositoryProvider).checkAvailability(),
);

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

  Future<void> measure(ArMeasureRequest request) async {
    if (state.isMeasuring) return;
    final previous = state.result;
    state = ArMeasureState(isMeasuring: true, result: previous);
    try {
      final result = await ref
          .read(arMeasureRepositoryProvider)
          .measureFloor(request);
      state = ArMeasureState(result: result ?? previous);
    } on ArMeasureFailure catch (e) {
      state = ArMeasureState(result: previous, failure: e);
    }
    // ARCore may have been installed or updated during the attempt.
    ref.invalidate(arAvailabilityProvider);
  }
}

final arMeasureControllerProvider =
    NotifierProvider<ArMeasureController, ArMeasureState>(
      ArMeasureController.new,
    );
