import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/features/device_lab/domain/ar_measure_failure.dart';
import 'package:tan_erp_field/generated/pigeon/ar_measure_api.g.dart';

export 'package:tan_erp_field/generated/pigeon/ar_measure_api.g.dart'
    show
        ArAvailability,
        ArMeasureColors,
        ArMeasureRequest,
        ArMeasureResult,
        ArMeasureStrings;

/// Wraps the Pigeon AR measure channel (Android ARCore; iOS stub).
class ArMeasureRepository {
  ArMeasureRepository(this._api);

  final ArMeasureApi _api;

  Future<ArAvailability> checkAvailability() async {
    try {
      return await _api.checkAvailability();
    } on PlatformException {
      return ArAvailability.unknown;
    }
  }

  /// Null when the user cancels. Throws [ArMeasureFailure].
  Future<ArMeasureResult?> measureFloor(ArMeasureRequest request) async {
    try {
      return await _api.measureFloor(request);
    } on PlatformException catch (e) {
      throw switch (e.code) {
        'camera_permission_denied' => ArMeasureFailure.cameraPermissionDenied,
        'install_declined' => ArMeasureFailure.installDeclined,
        'unavailable' => ArMeasureFailure.unavailable,
        'session_failed' => ArMeasureFailure.sessionFailed,
        'busy' => ArMeasureFailure.busy,
        _ => ArMeasureFailure.unknown,
      };
    }
  }
}

final arMeasureRepositoryProvider = Provider<ArMeasureRepository>(
  (ref) => ArMeasureRepository(ArMeasureApi()),
);
