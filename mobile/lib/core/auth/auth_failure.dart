import 'package:flutter/foundation.dart';

/// Sign-in failures. Chosen by Firebase error `code`, never by message.
@immutable
sealed class AuthFailure implements Exception {
  const AuthFailure();

  /// Maps a `FirebaseAuthException.code`.
  factory AuthFailure.fromFirebaseCode(String code) => switch (code) {
    'invalid-credential' ||
    'wrong-password' ||
    'user-not-found' ||
    'invalid-email' => const InvalidCredentialsFailure(),
    'user-disabled' => const AccountDisabledFailure(),
    'too-many-requests' => const TooManyAttemptsFailure(),
    'network-request-failed' => const AuthNetworkFailure(),
    _ => const AuthUnknownFailure(),
  };
}

class InvalidCredentialsFailure extends AuthFailure {
  const InvalidCredentialsFailure();
}

class AccountDisabledFailure extends AuthFailure {
  const AccountDisabledFailure();
}

class TooManyAttemptsFailure extends AuthFailure {
  const TooManyAttemptsFailure();
}

class AuthNetworkFailure extends AuthFailure {
  const AuthNetworkFailure();
}

class BiometricFailedFailure extends AuthFailure {
  const BiometricFailedFailure();
}

/// Firebase options are missing for this flavor.
class AuthNotConfiguredFailure extends AuthFailure {
  const AuthNotConfiguredFailure();
}

class AuthUnknownFailure extends AuthFailure {
  const AuthUnknownFailure();
}
