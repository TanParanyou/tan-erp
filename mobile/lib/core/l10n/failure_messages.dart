import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/l10n/gen/app_localizations.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';

/// User-facing text for any failure, chosen by error code / type only.
String failureMessage(AppLocalizations l10n, Object error) => switch (error) {
  InvalidCredentialsFailure() => l10n.errorInvalidCredentials,
  AccountDisabledFailure() => l10n.errorAccountDisabled,
  TooManyAttemptsFailure() => l10n.errorTooManyAttempts,
  AuthNetworkFailure() => l10n.errorNetwork,
  BiometricFailedFailure() => l10n.errorBiometricFailed,
  AuthFailure() => l10n.errorUnknown,
  ApiFailure(code: 'USER_ACCESS_DISABLED') => l10n.errorAccountDisabled,
  ApiFailure(code: 'ACTIVE_MEMBERSHIP_REQUIRED') => l10n.errorNoMembership,
  NetworkFailure() => l10n.errorNetwork,
  UnauthorizedFailure() => l10n.errorUnauthorized,
  ForbiddenFailure() => l10n.errorForbidden,
  NotFoundFailure() => l10n.errorNotFound,
  ConflictFailure() => l10n.errorConflict,
  ValidationFailure() => l10n.errorValidation,
  _ => l10n.errorUnknown,
};
