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

  @override
  String get syncBannerOffline => 'Offline. Changes are saved on this device';

  @override
  String syncBannerSyncing(int count) {
    return 'Syncing ($count)';
  }

  @override
  String syncBannerPending(int count) {
    return '$count changes waiting to sync';
  }

  @override
  String syncBannerConflict(int count) {
    return '$count sync conflicts found. Tap to review';
  }

  @override
  String get syncQueueTitle => 'Sync queue';

  @override
  String get syncQueueEmpty => 'All changes are synced';

  @override
  String get syncQueueSendNow => 'Send all now';

  @override
  String get syncQueueClearFailed => 'Remove failed item';

  @override
  String get syncStatusPending => 'Queued';

  @override
  String get syncStatusSyncing => 'Syncing';

  @override
  String get syncStatusConflict => 'Conflict';

  @override
  String get syncStatusFailed => 'Failed';

  @override
  String get numpadClear => 'Clear';

  @override
  String get numpadBackspace => 'Delete';

  @override
  String get numpadDone => 'Done';

  @override
  String get todayTitle => 'Today';

  @override
  String get todayRefresh => 'Refresh';

  @override
  String get todaySearchPlaceholder =>
      'Search opportunity code, customer or site';

  @override
  String get todayFilterAll => 'All';

  @override
  String get todayFilterSurveying => 'Surveying';

  @override
  String get todayFilterQualified => 'Ready to schedule';

  @override
  String get todayEmptyTitle => 'No surveys scheduled today';

  @override
  String get todayEmptyBody =>
      'There are no survey tasks in this branch yet. Tap to refresh';

  @override
  String get todayOpenSurvey => 'Open survey';

  @override
  String get todayStartSurvey => 'Start field survey';

  @override
  String get todayStageQualified => 'Qualified';

  @override
  String get todayStageSurveying => 'Surveying';

  @override
  String get todayStageEstimating => 'Estimating';

  @override
  String get todayNoSite => 'No site assigned';

  @override
  String todayScheduledAt(String date) {
    return 'Scheduled $date';
  }

  @override
  String get surveyListTitle => 'Field surveys';

  @override
  String get surveySearchPlaceholder =>
      'Search survey number or opportunity title';

  @override
  String get surveyEmptyTitle => 'No site surveys yet';

  @override
  String get surveyEmptyBody =>
      'Pick an opportunity from Today to start recording measurements';

  @override
  String get surveyGoToToday => 'Go to Today';

  @override
  String get surveyStatusDraft => 'Draft';

  @override
  String get surveyStatusReady => 'Ready for estimate';

  @override
  String get surveyStatusVoided => 'Voided';

  @override
  String get surveyStatusScheduled => 'Scheduled';

  @override
  String surveyDetailTitle(String number) {
    return 'Survey $number';
  }

  @override
  String get surveyScopeLabel => 'Scope summary';

  @override
  String get surveyScopePlaceholder =>
      'Describe the site scope or customer requirements';

  @override
  String get surveyAreasSection => 'Survey areas';

  @override
  String get surveySelectArea => 'Select area';

  @override
  String get surveyBreakdownMeasurements => 'Breakdown measurements';

  @override
  String get surveyTabletInspectorTitle => 'Survey & Spec Summary';

  @override
  String get surveyAddArea => 'Add area';

  @override
  String get surveyAreaNameLabel => 'Area name';

  @override
  String get surveyAreaNamePlaceholder => 'e.g. Ground floor hall, Kitchen';

  @override
  String get surveyAreaCodeLabel => 'Area code';

  @override
  String get surveyAreaCodePlaceholder => 'e.g. A-01';

  @override
  String get surveyAreaNotesLabel => 'Area notes';

  @override
  String get surveyAreaNotesPlaceholder => 'Site constraints or remarks';

  @override
  String get surveyDeleteArea => 'Delete area';

  @override
  String get surveyDeleteAreaConfirm =>
      'Delete this area and all of its measurements?';

  @override
  String get surveyMeasurementsSection => 'Measurements';

  @override
  String get surveyAddMeasurement => 'Add measurement';

  @override
  String get surveyMeasureTypeArea => 'Area';

  @override
  String get surveyMeasureTypeLength => 'Length';

  @override
  String get surveyMeasureTypeWidth => 'Width';

  @override
  String get surveyMeasureTypeHeight => 'Height';

  @override
  String get surveyMeasureTypeCount => 'Count';

  @override
  String get surveyUnitSqm => 'm²';

  @override
  String get surveyUnitM => 'm';

  @override
  String get surveyUnitMm => 'mm';

  @override
  String get surveyUnitPcs => 'pcs';

  @override
  String get surveyCaptureMeasured => 'Measured';

  @override
  String get surveyCapturePhoto => 'Photo A4';

  @override
  String get surveyCaptureAr => 'AR floor';

  @override
  String get surveyCaptureCustomer => 'Customer provided';

  @override
  String get surveyMeasureByPad => 'Keypad';

  @override
  String get surveyMeasureByPhoto => 'Photo A4';

  @override
  String get surveyMeasureByAr => 'AR floor';

  @override
  String get surveyMeasureNotesLabel => 'Measurement notes';

  @override
  String get surveyMeasureNotesPlaceholder =>
      'e.g. Deduct 2 columns, includes skirting';

  @override
  String get surveySaveDraft => 'Save draft';

  @override
  String get surveySavedOffline =>
      'Saved to offline queue. Will sync when online';

  @override
  String get surveySavedOnline => 'Survey draft saved';

  @override
  String get surveyMarkReady => 'Mark ready for estimate';

  @override
  String get surveyMarkReadyConfirm =>
      'Once marked ready, this revision is locked. Continue?';

  @override
  String get surveyNoAreas =>
      'No survey areas yet. Tap Add area to begin measuring';

  @override
  String get scanTitle => 'Scan item';

  @override
  String get scanSearchLabel => 'Barcode or item code';

  @override
  String get scanSearchPlaceholder => 'Enter barcode or SKU e.g. ITM-0001';

  @override
  String get scanSearchAction => 'Search';

  @override
  String get scanToggleCamera => 'Open camera scanner';

  @override
  String get scanCloseCamera => 'Close camera';

  @override
  String get scanCameraHint =>
      'Align the frame with the item barcode or QR code';

  @override
  String get scanResultTitle => 'Item details';

  @override
  String get scanItemCode => 'Item code';

  @override
  String get scanItemType => 'Item type';

  @override
  String get scanBaseUnit => 'Base unit';

  @override
  String get scanBarcodeValue => 'Barcode';

  @override
  String get scanPackaging => 'Packaging level';

  @override
  String get scanStockSection => 'Stock balance by warehouse';

  @override
  String scanStockAvailable(String qty, String unit) {
    return 'Available $qty $unit';
  }

  @override
  String scanStockOnHand(String onHand, String reserved) {
    return 'On hand $onHand, Reserved $reserved';
  }

  @override
  String get scanStockEmpty => 'No stock balances found in this branch';

  @override
  String get scanRecentTitle => 'Recent scans';

  @override
  String get scanEmptyTitle => 'Ready to scan or search';

  @override
  String get scanEmptyBody =>
      'Scan a barcode with the camera or enter an item code above to inspect item details and stock';

  @override
  String get scanNotFound => 'No item matches this code';

  @override
  String get notificationsTitle => 'Alerts';

  @override
  String get notificationsFilterAll => 'All';

  @override
  String get notificationsFilterUnread => 'Unread';

  @override
  String get notificationsMarkAllRead => 'Mark all read';

  @override
  String get notificationsMarkRead => 'Mark as read';

  @override
  String get notificationsEmptyTitle => 'No notifications';

  @override
  String get notificationsEmptyBody =>
      'Pending approvals and relevant updates will appear here';

  @override
  String get notificationsUnreadBadge => 'New';

  @override
  String get notificationsReadBadge => 'Read';

  @override
  String get notificationsTypeEstimate => 'Official estimate';

  @override
  String get notificationsTypeCost => 'Item cost';

  @override
  String get notificationsTypePo => 'Purchase order';

  @override
  String get notificationsTypeChangeOrder => 'Change order';

  @override
  String get notificationsTypeMrp => 'MRP plan';

  @override
  String get notificationsTypeRole => 'Role permission';

  @override
  String get notificationsTypeMembership => 'Branch membership';

  @override
  String get notificationsTypeGeneral => 'System alert';

  @override
  String get attachmentSectionTitle => 'Area photos & evidence';

  @override
  String get attachmentTakePhoto => 'Take photo';

  @override
  String get attachmentPickGallery => 'Gallery';

  @override
  String get attachmentEmptyHint => 'No photos attached to this area yet';

  @override
  String get attachmentStatusPending => 'Pending sync';

  @override
  String get attachmentStatusSynced => 'Synced';

  @override
  String get attachmentDeleteTitle => 'Delete photo';

  @override
  String get attachmentDeleteConfirm =>
      'Remove this photo from the survey area?';

  @override
  String get surveyCaptureLaser => 'BLE laser meter';

  @override
  String get surveyMeasureByLaser => 'Measure with Bluetooth laser';

  @override
  String get laserSheetTitle => 'Bluetooth laser distance meter';

  @override
  String get laserScanStart => 'Scan for laser meters';

  @override
  String get laserScanStop => 'Stop scan';

  @override
  String get laserScanning => 'Scanning for BLE devices...';

  @override
  String get laserNoDevices => 'No laser distance meters found nearby';

  @override
  String get laserConnect => 'Connect';

  @override
  String get laserDisconnect => 'Disconnect';

  @override
  String laserConnectedLabel(String deviceName) {
    return 'Connected: $deviceName';
  }

  @override
  String get laserLatestReading => 'Latest laser reading';

  @override
  String get laserUseReading => 'Use this reading';

  @override
  String get laserSimulateButton => 'Simulate laser reading (test mode)';

  @override
  String get laserSimulatedDeviceName => 'DISTO-SIM (BLE Simulator)';

  @override
  String get laserBluetoothOff =>
      'Bluetooth is off or unavailable on this device. You can use the simulator button to test.';

  @override
  String get pushSectionTitle => 'Push notifications';

  @override
  String get pushStatusEnabled => 'Push notifications enabled';

  @override
  String get pushStatusDisabled => 'Push notifications not enabled';

  @override
  String get pushStatusUnavailable =>
      'Firebase Push is not configured on this device';

  @override
  String get pushEnableAction => 'Enable push notifications';

  @override
  String get pushTokenLabel => 'Device token (FCM)';

  @override
  String get quickEstimateOpenButton => 'Field quick estimate';

  @override
  String quickEstimateTitle(String number) {
    return 'Quick estimate ($number)';
  }

  @override
  String get quickEstimateCreateAction => 'Create quick estimate';

  @override
  String get quickEstimateTemplateLabel => 'Pricing template';

  @override
  String get quickEstimatePropertyTypeLabel => 'Property type';

  @override
  String get quickEstimatePropertyCondo => 'Condominium';

  @override
  String get quickEstimatePropertyHouse => 'Detached house / Townhome';

  @override
  String get quickEstimatePropertyOffice => 'Office / Commercial';

  @override
  String get quickEstimateGradeLabel => 'Material grade';

  @override
  String get quickEstimateGradeStandard => 'Standard';

  @override
  String get quickEstimateGradePremium => 'Premium';

  @override
  String get quickEstimateGradeLuxury => 'Luxury';

  @override
  String get quickEstimateCalculateAction => 'Calculate price range';

  @override
  String get quickEstimateRangeLabel => 'Estimated price range (incl. tax)';

  @override
  String get quickEstimateNetAmountLabel => 'Net amount';

  @override
  String get quickEstimateTaxAmountLabel => 'VAT 7%';

  @override
  String quickEstimateValidUntilLabel(String date) {
    return 'Valid until $date';
  }

  @override
  String get quickEstimateShareAction => 'Show customer share screen';

  @override
  String get quickEstimateShareSheetTitle =>
      'Preliminary price range for customer';

  @override
  String get quickEstimateShareDisclaimer =>
      'This is a preliminary range based on on-site survey measurements. Official pricing follows BOQ verification.';

  @override
  String get quickEstimateShareConfirmAction => 'Record customer share';

  @override
  String get quickEstimateShareSuccess => 'Customer share recorded';

  @override
  String get quickEstimateConvertAction =>
      'Convert to official estimate draft (BOQ)';

  @override
  String get quickEstimateConvertConfirmTitle =>
      'Convert to official estimate draft?';

  @override
  String get quickEstimateConvertConfirmBody =>
      'This will create an official estimate draft linked to this survey revision and measurements.';

  @override
  String get quickEstimateConvertSuccess => 'Official estimate draft created';

  @override
  String get quickEstimateLinesSection => 'Surveyed lines & quantities';

  @override
  String get approvalSheetTitle => 'Document approval review';

  @override
  String get approvalDocumentLabel => 'Document number';

  @override
  String get approvalSummaryLabel => 'Summary';

  @override
  String get approvalNoteLabel => 'Review note';

  @override
  String get approvalNotePlaceholder =>
      'Enter reason or note (required when returning/rejecting)...';

  @override
  String get approvalApproveAction => 'Approve document';

  @override
  String get approvalReturnAction => 'Return / Reject';

  @override
  String get approvalConfirmApproveTitle => 'Confirm document approval?';

  @override
  String get approvalConfirmReturnTitle =>
      'Confirm returning / rejecting document?';

  @override
  String get approvalSuccessApproved => 'Document approval recorded';

  @override
  String get approvalSuccessReturned => 'Document return recorded';

  @override
  String get approvalNoteRequiredError =>
      'Please enter a note before returning';

  @override
  String get approvalQuickActionHint => 'Tap to review & approve';

  @override
  String get conflictSheetTitle => 'Resolve sync conflict (409/412)';

  @override
  String get conflictDescription =>
      'Server data changed while you were working offline. Choose how to resolve this item.';

  @override
  String get conflictFieldColumn => 'Field';

  @override
  String get conflictLocalColumn => 'Your device';

  @override
  String get conflictServerColumn => 'Server state';

  @override
  String get conflictKeepLocalAction => 'Keep device changes (Retry)';

  @override
  String get conflictAcceptServerAction =>
      'Accept server version (Discard local)';

  @override
  String get conflictResolveAction => 'Inspect & resolve conflict';
}
