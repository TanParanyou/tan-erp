import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/l10n/gen/app_localizations.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';

/// User-facing text for any failure, chosen by error code / type only.
String failureMessage(AppLocalizations l10n, Object error) => switch (error) {
  InvalidCredentialsFailure() => l10n.errorInvalidCredentials,
  AccountLockedOutFailure() => l10n.errorAccountLockedOut,
  UserNotInvitedFailure() => l10n.errorUserNotInvited,
  AccountDisabledFailure() => l10n.errorAccountDisabled,
  TwoFactorCodeInvalidFailure() => l10n.errorTwoFactorCodeInvalid,
  TwoFactorChallengeInvalidFailure() => l10n.errorTwoFactorChallengeInvalid,
  SessionExpiredFailure() => l10n.errorUnauthorized,
  TooManyAttemptsFailure() => l10n.errorTooManyAttempts,
  AuthNetworkFailure() => l10n.errorNetwork,
  BiometricFailedFailure() => l10n.errorBiometricFailed,
  AuthNotConfiguredFailure() => l10n.notConfiguredBody,
  ApiFailure(code: 'INVALID_CREDENTIALS') => l10n.errorInvalidCredentials,
  ApiFailure(code: 'ACCOUNT_LOCKED_OUT') => l10n.errorAccountLockedOut,
  ApiFailure(code: 'USER_NOT_INVITED') => l10n.errorUserNotInvited,
  ApiFailure(code: 'USER_ACCESS_DISABLED') => l10n.errorAccountDisabled,
  ApiFailure(code: 'TWO_FACTOR_CODE_INVALID') => l10n.errorTwoFactorCodeInvalid,
  ApiFailure(code: 'TWO_FACTOR_CHALLENGE_INVALID') =>
    l10n.errorTwoFactorChallengeInvalid,
  ApiFailure(code: 'AUTHENTICATION_INVALID') => l10n.errorUnauthorized,
  ApiFailure(code: 'ACTIVE_MEMBERSHIP_REQUIRED') => l10n.errorNoMembership,
  NetworkFailure() => l10n.errorNetwork,
  UnauthorizedFailure() => l10n.errorUnauthorized,
  ForbiddenFailure() => l10n.errorForbidden,
  NotFoundFailure() => l10n.errorNotFound,
  ConflictFailure() => l10n.errorConflict,
  ValidationFailure() => l10n.errorValidation,
  AuthFailure() => l10n.errorUnknown,
  _ => l10n.errorUnknown,
};
