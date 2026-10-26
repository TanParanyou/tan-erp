// ignore: unused_import
import 'package:intl/intl.dart' as intl;

import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'TAN Field';

  @override
  String get commonCancel => 'Cancel';

  @override
  String get commonConfirm => 'Confirm';

  @override
  String get commonRetry => 'Retry';

  @override
  String get commonSelect => 'Select';

  @override
  String get commonFieldRequired => 'This field is required';

  @override
  String get loginTitle => 'Sign in';

  @override
  String get loginEmailLabel => 'Email';

  @override
  String get loginEmailPlaceholder => 'name@company.com';

  @override
  String get loginPasswordLabel => 'Password';

  @override
  String get loginPasswordPlaceholder => 'Enter your password';

  @override
  String get loginSubmit => 'Sign in';

  @override
  String get notConfiguredTitle => 'Firebase is not configured';

  @override
  String get notConfiguredBody =>
      'Add the FIREBASE_* values to this flavor\'s config file and restart the app with --dart-define-from-file';

  @override
  String get unlockTitle => 'Unlock the app';

  @override
  String get unlockBody => 'Verify your identity to continue';

  @override
  String get unlockAction => 'Unlock';

  @override
  String get unlockReason => 'Verify your identity to open TAN Field';

  @override
  String get unlockUseOtherAccount => 'Use another account';

  @override
  String get membershipTitle => 'Choose a branch';

  @override
  String get membershipLabel => 'Working branch';

  @override
  String get membershipAllBranches => 'All branches';

  @override
  String get membershipContinue => 'Continue';

  @override
  String get navToday => 'Today';

  @override
  String get navSurvey => 'Survey';

  @override
  String get navScan => 'Scan';

  @override
  String get navNotifications => 'Alerts';

  @override
  String get navMe => 'Me';

  @override
  String get inDevelopmentTitle => 'Under development';

  @override
  String get inDevelopmentBody =>
      'This tab will be available in a later version';

  @override
  String get meTitle => 'Me';

  @override
  String get meProfile => 'Profile';

  @override
  String get meOrganization => 'Organization';

  @override
  String get meBranch => 'Branch';

  @override
  String get meSwitchBranch => 'Switch branch';

  @override
  String get meLanguage => 'Language';

  @override
  String get meAppVersion => 'App version';

  @override
  String get meLogout => 'Sign out';

  @override
  String get meUiGallery => 'UI gallery';

  @override
  String get languageThai => 'ไทย';

  @override
  String get languageEnglish => 'English';

  @override
  String get themeMode => 'Colour mode';

  @override
  String get themeModeSystem => 'System';

  @override
  String get themeModeLight => 'Light';

  @override
  String get themeModeDark => 'Dark';

  @override
  String get themeDensity => 'Density';

  @override
  String get themeDensityNormal => 'Normal';

  @override
  String get themeDensityCompact => 'Compact';

  @override
  String get themeTextScale => 'Text size';

  @override
  String themeTextScaleValue(int percent) {
    return '$percent%';
  }

  @override
  String get logoutConfirmTitle => 'Sign out';

  @override
  String get logoutConfirmBody => 'Do you want to sign out?';

  @override
  String get forceUpdateTitle => 'Update required';

  @override
  String forceUpdateBody(String minimum) {
    return 'This version is no longer supported. Please update to version $minimum or later';
  }

  @override
  String forceUpdateCurrent(String current) {
    return 'Current version $current';
  }

  @override
  String get errorInvalidCredentials => 'Incorrect email or password';

  @override
  String get errorAccountDisabled => 'This account has been disabled';

  @override
  String get errorNoMembership =>
      'This account has no access to any branch yet';

  @override
  String get errorNetwork => 'Cannot reach the network. Check your connection';

  @override
  String get errorTooManyAttempts =>
      'Too many sign-in attempts. Please wait a moment';

  @override
  String get errorUnauthorized => 'Your session expired. Please sign in again';

  @override
  String get errorForbidden => 'You do not have permission for this action';

  @override
  String get errorNotFound => 'Not found';

  @override
  String get errorConflict => 'This record was changed. Reload and try again';

  @override
  String get errorValidation =>
      'Some data is invalid. Please check and try again';

  @override
  String get errorBiometricFailed => 'Verification failed';

  @override
  String get errorUnknown => 'Something went wrong. Please try again';

  @override
  String get galleryTitle => 'UI gallery';

  @override
  String get galleryPrimaryButton => 'Primary button';

  @override
  String get galleryOutlineButton => 'Outline button';

  @override
  String get galleryGhostButton => 'Ghost button';

  @override
  String get galleryLoadingButton => 'Saving';

  @override
  String get galleryTextField => 'Text field';

  @override
  String get galleryTextFieldPlaceholder => 'Type something';

  @override
  String get gallerySelect => 'Select';

  @override
  String get galleryAlertInfo => 'General information';

  @override
  String get galleryAlertSuccess => 'Saved successfully';

  @override
  String get galleryAlertWarning => 'Please review before continuing';

  @override
  String get galleryAlertDanger => 'An error occurred';

  @override
  String get galleryConfirmOpen => 'Open confirm dialog';

  @override
  String get galleryConfirmMessage => 'Do you want to continue?';

  @override
  String get galleryEmptyTitle => 'No items';

  @override
  String get galleryEmptyMessage => 'There is nothing in this category yet';

  @override
  String get galleryEmptyAction => 'Add item';
}
