import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/features/device_lab/domain/ar_measure_failure.dart';
import 'package:tan_erp_field/generated/pigeon/ar_measure_api.g.dart';

export 'package:tan_erp_field/generated/pigeon/ar_measure_api.g.dart'
    show RoomScanRequest, RoomScanResult;

/// Wraps the Pigeon room scan channel (iOS RoomPlan only; Android has no
/// handler, so it reports unsupported).
class RoomScanService {
  RoomScanService(this._api);

  final RoomScanApi _api;

  Future<bool> isSupported() async {
    try {
      return await _api.isSupported();
    } on PlatformException {
      return false;
    }
  }

  /// Null when the user cancels. Throws [ArMeasureFailure].
  Future<RoomScanResult?> scanRoom(RoomScanRequest request) async {
    try {
      return await _api.scanRoom(request);
    } on PlatformException catch (e) {
      throw ArMeasureFailure.fromCode(e.code);
    }
  }
}

final roomScanServiceProvider = Provider<RoomScanService>(
  (ref) => RoomScanService(RoomScanApi()),
);

final roomScanSupportedProvider = FutureProvider<bool>(
  (ref) => ref.watch(roomScanServiceProvider).isSupported(),
);
