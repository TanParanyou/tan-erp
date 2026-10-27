/// Why the native AR measure could not finish; mapped from Pigeon error codes.
enum ArMeasureFailure implements Exception {
  cameraPermissionDenied,
  installDeclined,
  unavailable,
  sessionFailed,
  busy,
  unknown,
}
