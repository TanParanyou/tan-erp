/// Why a native AR measure or room scan could not finish; mapped from Pigeon
/// error codes.
enum ArMeasureFailure implements Exception {
  cameraPermissionDenied,
  installDeclined,
  unavailable,
  sessionFailed,
  noFloor,
  busy,
  unknown;

  /// Maps a PlatformException code from the native side.
  static ArMeasureFailure fromCode(String code) => switch (code) {
    'camera_permission_denied' => cameraPermissionDenied,
    'install_declined' => installDeclined,
    'unavailable' => unavailable,
    'session_failed' => sessionFailed,
    'no_floor' => noFloor,
    'busy' => busy,
    _ => unknown,
  };
}
