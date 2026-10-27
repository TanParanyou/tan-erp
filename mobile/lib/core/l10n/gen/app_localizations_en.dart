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
  String get loginMockBanner => 'Test mode (Mock login)';

  @override
  String get loginMockUidLabel => 'User ID (uid)';

  @override
  String get loginMockUidPlaceholder => 'e.g. foundation-user-test-only';

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

  @override
  String get galleryCardTitle => 'Card title';

  @override
  String get galleryCardBody => 'Card body text';

  @override
  String get deviceLabTitle => 'Device lab';

  @override
  String get deviceLabIntro =>
      'Try device-specific features. Results are not saved';

  @override
  String get arCardTitle => 'AR floor measure';

  @override
  String get arCardBody =>
      'Place points on the floor corners through the camera and the app calculates the area';

  @override
  String get arAvailabilitySupported => 'This device supports AR';

  @override
  String get arAvailabilityNeedsInstall =>
      'Google Play Services for AR must be installed or updated. You will be asked when you start';

  @override
  String get arAvailabilityUnsupported =>
      'This device does not support ARCore. Try a supported phone';

  @override
  String get arAvailabilityUnknown => 'Could not check AR support';

  @override
  String get arStart => 'Start measuring';

  @override
  String get arResultTitle => 'Latest measurement';

  @override
  String get arResultArea => 'Area';

  @override
  String get arResultPerimeter => 'Perimeter';

  @override
  String arResultEdge(int index) {
    return 'Side $index';
  }

  @override
  String arValueMeters(String value) {
    return '$value m';
  }

  @override
  String arValueSquareMeters(String value) {
    return '$value m²';
  }

  @override
  String get arUnitMeter => 'm';

  @override
  String get arUnitSquareMeter => 'm²';

  @override
  String get arSearchingPlane =>
      'Point the camera at the floor and move the phone slowly';

  @override
  String get arTapToPlace => 'Tap the floor to place the first point';

  @override
  String get arAddMorePoints =>
      'Tap to place the next point. At least 3 points are needed';

  @override
  String get arReadyToClose => 'When every corner is placed, tap Close shape';

  @override
  String get arClosedHint => 'Check the shape, then tap Done';

  @override
  String get arTrackingLost =>
      'Tracking lost. Move slower and point at a textured floor';

  @override
  String get arNotOnPlane => 'No floor found where you tapped';

  @override
  String get arSelfIntersecting =>
      'Lines cross. Tap Undo and place the points again';

  @override
  String get arUndo => 'Undo';

  @override
  String get arCloseShape => 'Close shape';

  @override
  String get arDone => 'Done';

  @override
  String get arErrorCameraPermission =>
      'Camera access was denied. Allow it in the device settings';

  @override
  String get arErrorInstallDeclined =>
      'Google Play Services for AR is not installed';

  @override
  String get arErrorUnavailable => 'AR is not available on this device';

  @override
  String get arErrorSessionFailed =>
      'Could not start the AR camera. Please try again';

  @override
  String get arErrorBusy => 'A measurement is already running';

  @override
  String get photoAreaCardTitle => 'Measure area from photo + reference sheet';

  @override
  String get photoAreaCardBody =>
      'Take a photo of the floor with a sheet of known size lying on it, tap the sheet corners and the area corners';

  @override
  String get photoAreaReferenceLabel => 'Reference sheet';

  @override
  String get photoAreaReferencePlaceholder => 'Choose a reference size';

  @override
  String get photoAreaPresetA4 => 'A4 (210 × 297 mm)';

  @override
  String get photoAreaPresetA3 => 'A3 (297 × 420 mm)';

  @override
  String get photoAreaPresetCustom => 'Custom size';

  @override
  String get photoAreaWidthLabel => 'Width (mm)';

  @override
  String get photoAreaWidthPlaceholder => 'e.g. 210';

  @override
  String get photoAreaHeightLabel => 'Height (mm)';

  @override
  String get photoAreaHeightPlaceholder => 'e.g. 297';

  @override
  String get photoAreaErrorSize => 'Enter a width and height greater than 0';

  @override
  String get photoAreaPickGallery => 'Choose from gallery';

  @override
  String get photoAreaPickCamera => 'Take a photo';

  @override
  String get photoAreaErrorPick => 'Could not open the photo. Please try again';

  @override
  String get photoAreaTitle => 'Measure from photo';

  @override
  String photoAreaStepReference(int count) {
    return 'Tap the 4 corners of the reference sheet ($count/4)';
  }

  @override
  String photoAreaStepArea(int count) {
    return 'Tap the corners of the area, at least 3 ($count)';
  }

  @override
  String get photoAreaStepAdjust => 'Drag a point to adjust it';

  @override
  String get photoAreaClear => 'Clear';

  @override
  String get photoAreaWidth => 'Width';

  @override
  String get photoAreaLength => 'Length';

  @override
  String get photoAreaVertices => 'Points';

  @override
  String photoAreaValueMm(String value) {
    return '$value mm';
  }

  @override
  String get photoAreaErrorReference =>
      'The reference corners are invalid. Move them onto the four corners of the sheet';
}
