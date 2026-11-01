import 'package:flutter/foundation.dart';

/// Sign-in and authentication failures mapped from backend error codes.
@immutable
sealed class AuthFailure implements Exception {
  const AuthFailure();

  /// Maps a backend RFC 9457 error code.
  factory AuthFailure.fromCode(String? code) => switch (code) {
    'INVALID_CREDENTIALS' => const InvalidCredentialsFailure(),
    'ACCOUNT_LOCKED_OUT' => const AccountLockedOutFailure(),
    'USER_NOT_INVITED' => const UserNotInvitedFailure(),
    'USER_ACCESS_DISABLED' => const AccountDisabledFailure(),
    'TWO_FACTOR_CODE_INVALID' => const TwoFactorCodeInvalidFailure(),
    'TWO_FACTOR_CHALLENGE_INVALID' => const TwoFactorChallengeInvalidFailure(),
    'AUTHENTICATION_INVALID' => const SessionExpiredFailure(),
    'TOO_MANY_ATTEMPTS' => const TooManyAttemptsFailure(),
    _ => const AuthUnknownFailure(),
  };
}

class InvalidCredentialsFailure extends AuthFailure {
  const InvalidCredentialsFailure();
}

class AccountLockedOutFailure extends AuthFailure {
  const AccountLockedOutFailure();
}

class UserNotInvitedFailure extends AuthFailure {
  const UserNotInvitedFailure();
}

class AccountDisabledFailure extends AuthFailure {
  const AccountDisabledFailure();
}

class TwoFactorCodeInvalidFailure extends AuthFailure {
  const TwoFactorCodeInvalidFailure();
}

class TwoFactorChallengeInvalidFailure extends AuthFailure {
  const TwoFactorChallengeInvalidFailure();
}

class SessionExpiredFailure extends AuthFailure {
  const SessionExpiredFailure();
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

class AuthNotConfiguredFailure extends AuthFailure {
  const AuthNotConfiguredFailure();
}

class AuthUnknownFailure extends AuthFailure {
  const AuthUnknownFailure();
}
