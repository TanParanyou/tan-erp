import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:local_auth/local_auth.dart';

/// Biometric (or device passcode) gate used when reopening the app.
class BiometricGate {
  BiometricGate([LocalAuthentication? auth])
    : _auth = auth ?? LocalAuthentication();

  final LocalAuthentication _auth;

  /// Devices with no lock screen cannot be gated; callers skip the gate then.
  Future<bool> isAvailable() async {
    try {
      return await _auth.isDeviceSupported();
    } on LocalAuthException {
      return false;
    }
  }

  /// Biometric first, device passcode as fallback.
  Future<bool> authenticate(String reason) async {
    try {
      return await _auth.authenticate(localizedReason: reason);
    } on LocalAuthException {
      return false;
    }
  }
}

final biometricGateProvider = Provider<BiometricGate>((ref) => BiometricGate());
