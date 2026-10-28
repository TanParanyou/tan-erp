import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_en.dart';
import 'app_localizations_th.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'gen/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
    : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations)!;
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
        delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
      ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('en'),
    Locale('th'),
  ];

  /// No description provided for @appTitle.
  ///
  /// In th, this message translates to:
  /// **'TAN Field'**
  String get appTitle;

  /// No description provided for @commonCancel.
  ///
  /// In th, this message translates to:
  /// **'ยกเลิก'**
  String get commonCancel;

  /// No description provided for @commonConfirm.
  ///
  /// In th, this message translates to:
  /// **'ยืนยัน'**
  String get commonConfirm;

  /// No description provided for @commonRetry.
  ///
  /// In th, this message translates to:
  /// **'ลองใหม่'**
  String get commonRetry;

  /// No description provided for @commonSelect.
  ///
  /// In th, this message translates to:
  /// **'เลือก'**
  String get commonSelect;

  /// No description provided for @commonFieldRequired.
  ///
  /// In th, this message translates to:
  /// **'กรุณากรอกข้อมูล'**
  String get commonFieldRequired;

  /// No description provided for @loginTitle.
  ///
  /// In th, this message translates to:
  /// **'เข้าสู่ระบบ'**
  String get loginTitle;

  /// No description provided for @loginEmailLabel.
  ///
  /// In th, this message translates to:
  /// **'อีเมล'**
  String get loginEmailLabel;

  /// No description provided for @loginEmailPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'name@company.com'**
  String get loginEmailPlaceholder;

  /// No description provided for @loginPasswordLabel.
  ///
  /// In th, this message translates to:
  /// **'รหัสผ่าน'**
  String get loginPasswordLabel;

  /// No description provided for @loginPasswordPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'กรอกรหัสผ่าน'**
  String get loginPasswordPlaceholder;

  /// No description provided for @loginMockBanner.
  ///
  /// In th, this message translates to:
  /// **'โหมดทดสอบ (Mock login)'**
  String get loginMockBanner;

  /// No description provided for @loginMockUidLabel.
  ///
  /// In th, this message translates to:
  /// **'รหัสผู้ใช้ (uid)'**
  String get loginMockUidLabel;

  /// No description provided for @loginMockUidPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'เช่น foundation-user-test-only'**
  String get loginMockUidPlaceholder;

  /// No description provided for @loginSubmit.
  ///
  /// In th, this message translates to:
  /// **'เข้าสู่ระบบ'**
  String get loginSubmit;

  /// No description provided for @notConfiguredTitle.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่ได้ตั้งค่า Firebase'**
  String get notConfiguredTitle;

  /// No description provided for @notConfiguredBody.
  ///
  /// In th, this message translates to:
  /// **'เพิ่มค่า FIREBASE_* ในไฟล์ config ของ flavor นี้ แล้วรันแอปใหม่ด้วย --dart-define-from-file'**
  String get notConfiguredBody;

  /// No description provided for @unlockTitle.
  ///
  /// In th, this message translates to:
  /// **'ปลดล็อกแอป'**
  String get unlockTitle;

  /// No description provided for @unlockBody.
  ///
  /// In th, this message translates to:
  /// **'ยืนยันตัวตนเพื่อเข้าใช้งานต่อ'**
  String get unlockBody;

  /// No description provided for @unlockAction.
  ///
  /// In th, this message translates to:
  /// **'ปลดล็อก'**
  String get unlockAction;

  /// No description provided for @unlockReason.
  ///
  /// In th, this message translates to:
  /// **'ยืนยันตัวตนเพื่อเข้าใช้ TAN Field'**
  String get unlockReason;

  /// No description provided for @unlockUseOtherAccount.
  ///
  /// In th, this message translates to:
  /// **'ใช้บัญชีอื่น'**
  String get unlockUseOtherAccount;

  /// No description provided for @membershipTitle.
  ///
  /// In th, this message translates to:
  /// **'เลือกสาขา'**
  String get membershipTitle;

  /// No description provided for @membershipLabel.
  ///
  /// In th, this message translates to:
  /// **'สาขาที่ทำงาน'**
  String get membershipLabel;

  /// No description provided for @membershipAllBranches.
  ///
  /// In th, this message translates to:
  /// **'ทุกสาขา'**
  String get membershipAllBranches;

  /// No description provided for @membershipContinue.
  ///
  /// In th, this message translates to:
  /// **'เข้าใช้งาน'**
  String get membershipContinue;

  /// No description provided for @navToday.
  ///
  /// In th, this message translates to:
  /// **'งานวันนี้'**
  String get navToday;

  /// No description provided for @navSurvey.
  ///
  /// In th, this message translates to:
  /// **'สำรวจ'**
  String get navSurvey;

  /// No description provided for @navScan.
  ///
  /// In th, this message translates to:
  /// **'สแกน'**
  String get navScan;

  /// No description provided for @navNotifications.
  ///
  /// In th, this message translates to:
  /// **'แจ้งเตือน'**
  String get navNotifications;

  /// No description provided for @navMe.
  ///
  /// In th, this message translates to:
  /// **'ฉัน'**
  String get navMe;

  /// No description provided for @inDevelopmentTitle.
  ///
  /// In th, this message translates to:
  /// **'กำลังพัฒนา'**
  String get inDevelopmentTitle;

  /// No description provided for @inDevelopmentBody.
  ///
  /// In th, this message translates to:
  /// **'เมนูนี้จะเปิดให้ใช้งานในเวอร์ชันถัดไป'**
  String get inDevelopmentBody;

  /// No description provided for @meTitle.
  ///
  /// In th, this message translates to:
  /// **'ฉัน'**
  String get meTitle;

  /// No description provided for @meProfile.
  ///
  /// In th, this message translates to:
  /// **'โปรไฟล์'**
  String get meProfile;

  /// No description provided for @meOrganization.
  ///
  /// In th, this message translates to:
  /// **'องค์กร'**
  String get meOrganization;

  /// No description provided for @meBranch.
  ///
  /// In th, this message translates to:
  /// **'สาขา'**
  String get meBranch;

  /// No description provided for @meSwitchBranch.
  ///
  /// In th, this message translates to:
  /// **'เปลี่ยนสาขา'**
  String get meSwitchBranch;

  /// No description provided for @meLanguage.
  ///
  /// In th, this message translates to:
  /// **'ภาษา'**
  String get meLanguage;

  /// No description provided for @meAppVersion.
  ///
  /// In th, this message translates to:
  /// **'เวอร์ชันแอป'**
  String get meAppVersion;

  /// No description provided for @meLogout.
  ///
  /// In th, this message translates to:
  /// **'ออกจากระบบ'**
  String get meLogout;

  /// No description provided for @meUiGallery.
  ///
  /// In th, this message translates to:
  /// **'ตัวอย่างคอมโพเนนต์'**
  String get meUiGallery;

  /// No description provided for @languageThai.
  ///
  /// In th, this message translates to:
  /// **'ไทย'**
  String get languageThai;

  /// No description provided for @languageEnglish.
  ///
  /// In th, this message translates to:
  /// **'English'**
  String get languageEnglish;

  /// No description provided for @themeMode.
  ///
  /// In th, this message translates to:
  /// **'โหมดสี'**
  String get themeMode;

  /// No description provided for @themeModeSystem.
  ///
  /// In th, this message translates to:
  /// **'ตามเครื่อง'**
  String get themeModeSystem;

  /// No description provided for @themeModeLight.
  ///
  /// In th, this message translates to:
  /// **'สว่าง'**
  String get themeModeLight;

  /// No description provided for @themeModeDark.
  ///
  /// In th, this message translates to:
  /// **'มืด'**
  String get themeModeDark;

  /// No description provided for @themeDensity.
  ///
  /// In th, this message translates to:
  /// **'ความแน่น'**
  String get themeDensity;

  /// No description provided for @themeDensityNormal.
  ///
  /// In th, this message translates to:
  /// **'ปกติ'**
  String get themeDensityNormal;

  /// No description provided for @themeDensityCompact.
  ///
  /// In th, this message translates to:
  /// **'กะทัดรัด'**
  String get themeDensityCompact;

  /// No description provided for @themeTextScale.
  ///
  /// In th, this message translates to:
  /// **'ขนาดตัวอักษร'**
  String get themeTextScale;

  /// No description provided for @themeTextScaleValue.
  ///
  /// In th, this message translates to:
  /// **'{percent}%'**
  String themeTextScaleValue(int percent);

  /// No description provided for @logoutConfirmTitle.
  ///
  /// In th, this message translates to:
  /// **'ออกจากระบบ'**
  String get logoutConfirmTitle;

  /// No description provided for @logoutConfirmBody.
  ///
  /// In th, this message translates to:
  /// **'ต้องการออกจากระบบใช่หรือไม่'**
  String get logoutConfirmBody;

  /// No description provided for @forceUpdateTitle.
  ///
  /// In th, this message translates to:
  /// **'ต้องอัปเดตแอป'**
  String get forceUpdateTitle;

  /// No description provided for @forceUpdateBody.
  ///
  /// In th, this message translates to:
  /// **'เวอร์ชันนี้ไม่รองรับแล้ว กรุณาอัปเดตเป็นเวอร์ชัน {minimum} ขึ้นไป'**
  String forceUpdateBody(String minimum);

  /// No description provided for @forceUpdateCurrent.
  ///
  /// In th, this message translates to:
  /// **'เวอร์ชันปัจจุบัน {current}'**
  String forceUpdateCurrent(String current);

  /// No description provided for @errorInvalidCredentials.
  ///
  /// In th, this message translates to:
  /// **'อีเมลหรือรหัสผ่านไม่ถูกต้อง'**
  String get errorInvalidCredentials;

  /// No description provided for @errorAccountDisabled.
  ///
  /// In th, this message translates to:
  /// **'บัญชีนี้ถูกปิดการใช้งาน'**
  String get errorAccountDisabled;

  /// No description provided for @errorNoMembership.
  ///
  /// In th, this message translates to:
  /// **'บัญชีนี้ยังไม่มีสิทธิ์เข้าใช้งานสาขาใด'**
  String get errorNoMembership;

  /// No description provided for @errorNetwork.
  ///
  /// In th, this message translates to:
  /// **'เชื่อมต่อเครือข่ายไม่ได้ กรุณาตรวจสอบอินเทอร์เน็ต'**
  String get errorNetwork;

  /// No description provided for @errorTooManyAttempts.
  ///
  /// In th, this message translates to:
  /// **'ลองเข้าสู่ระบบหลายครั้งเกินไป กรุณารอสักครู่'**
  String get errorTooManyAttempts;

  /// No description provided for @errorUnauthorized.
  ///
  /// In th, this message translates to:
  /// **'เซสชันหมดอายุ กรุณาเข้าสู่ระบบใหม่'**
  String get errorUnauthorized;

  /// No description provided for @errorForbidden.
  ///
  /// In th, this message translates to:
  /// **'ไม่มีสิทธิ์ทำรายการนี้'**
  String get errorForbidden;

  /// No description provided for @errorNotFound.
  ///
  /// In th, this message translates to:
  /// **'ไม่พบข้อมูล'**
  String get errorNotFound;

  /// No description provided for @errorConflict.
  ///
  /// In th, this message translates to:
  /// **'ข้อมูลถูกแก้ไขแล้ว กรุณาโหลดใหม่'**
  String get errorConflict;

  /// No description provided for @errorValidation.
  ///
  /// In th, this message translates to:
  /// **'ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง'**
  String get errorValidation;

  /// No description provided for @errorBiometricFailed.
  ///
  /// In th, this message translates to:
  /// **'ยืนยันตัวตนไม่สำเร็จ'**
  String get errorBiometricFailed;

  /// No description provided for @errorUnknown.
  ///
  /// In th, this message translates to:
  /// **'เกิดข้อผิดพลาด กรุณาลองใหม่'**
  String get errorUnknown;

  /// No description provided for @galleryTitle.
  ///
  /// In th, this message translates to:
  /// **'ตัวอย่างคอมโพเนนต์'**
  String get galleryTitle;

  /// No description provided for @galleryPrimaryButton.
  ///
  /// In th, this message translates to:
  /// **'ปุ่มหลัก'**
  String get galleryPrimaryButton;

  /// No description provided for @galleryOutlineButton.
  ///
  /// In th, this message translates to:
  /// **'ปุ่มรอง'**
  String get galleryOutlineButton;

  /// No description provided for @galleryGhostButton.
  ///
  /// In th, this message translates to:
  /// **'ปุ่มข้อความ'**
  String get galleryGhostButton;

  /// No description provided for @galleryLoadingButton.
  ///
  /// In th, this message translates to:
  /// **'กำลังบันทึก'**
  String get galleryLoadingButton;

  /// No description provided for @galleryTextField.
  ///
  /// In th, this message translates to:
  /// **'ช่องกรอกข้อมูล'**
  String get galleryTextField;

  /// No description provided for @galleryTextFieldPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'พิมพ์ข้อความ'**
  String get galleryTextFieldPlaceholder;

  /// No description provided for @gallerySelect.
  ///
  /// In th, this message translates to:
  /// **'ตัวเลือก'**
  String get gallerySelect;

  /// No description provided for @galleryAlertInfo.
  ///
  /// In th, this message translates to:
  /// **'ข้อมูลทั่วไป'**
  String get galleryAlertInfo;

  /// No description provided for @galleryAlertSuccess.
  ///
  /// In th, this message translates to:
  /// **'บันทึกสำเร็จ'**
  String get galleryAlertSuccess;

  /// No description provided for @galleryAlertWarning.
  ///
  /// In th, this message translates to:
  /// **'โปรดตรวจสอบก่อนดำเนินการ'**
  String get galleryAlertWarning;

  /// No description provided for @galleryAlertDanger.
  ///
  /// In th, this message translates to:
  /// **'เกิดข้อผิดพลาด'**
  String get galleryAlertDanger;

  /// No description provided for @galleryConfirmOpen.
  ///
  /// In th, this message translates to:
  /// **'เปิดกล่องยืนยัน'**
  String get galleryConfirmOpen;

  /// No description provided for @galleryConfirmMessage.
  ///
  /// In th, this message translates to:
  /// **'ต้องการดำเนินการต่อใช่หรือไม่'**
  String get galleryConfirmMessage;

  /// No description provided for @galleryEmptyTitle.
  ///
  /// In th, this message translates to:
  /// **'ไม่มีรายการ'**
  String get galleryEmptyTitle;

  /// No description provided for @galleryEmptyMessage.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่มีข้อมูลในหมวดนี้'**
  String get galleryEmptyMessage;

  /// No description provided for @galleryEmptyAction.
  ///
  /// In th, this message translates to:
  /// **'เพิ่มรายการ'**
  String get galleryEmptyAction;

  /// No description provided for @galleryCardTitle.
  ///
  /// In th, this message translates to:
  /// **'หัวการ์ด'**
  String get galleryCardTitle;

  /// No description provided for @galleryCardBody.
  ///
  /// In th, this message translates to:
  /// **'เนื้อหาในการ์ด'**
  String get galleryCardBody;

  /// No description provided for @deviceLabTitle.
  ///
  /// In th, this message translates to:
  /// **'ห้องทดลองอุปกรณ์'**
  String get deviceLabTitle;

  /// No description provided for @deviceLabIntro.
  ///
  /// In th, this message translates to:
  /// **'ทดลองความสามารถเฉพาะของเครื่อง ผลลัพธ์ไม่ถูกบันทึกลงระบบ'**
  String get deviceLabIntro;

  /// No description provided for @arCardTitle.
  ///
  /// In th, this message translates to:
  /// **'AR วัดพื้น'**
  String get arCardTitle;

  /// No description provided for @arCardBody.
  ///
  /// In th, this message translates to:
  /// **'วางจุดตามมุมห้องบนพื้นผ่านกล้อง แล้วแอปคำนวณพื้นที่ให้'**
  String get arCardBody;

  /// No description provided for @arAvailabilitySupported.
  ///
  /// In th, this message translates to:
  /// **'เครื่องนี้รองรับ AR'**
  String get arAvailabilitySupported;

  /// No description provided for @arAvailabilityNeedsInstall.
  ///
  /// In th, this message translates to:
  /// **'ต้องติดตั้งหรืออัปเดต Google Play Services for AR ระบบจะถามเมื่อเริ่มวัด'**
  String get arAvailabilityNeedsInstall;

  /// No description provided for @arAvailabilityUnsupported.
  ///
  /// In th, this message translates to:
  /// **'เครื่องนี้ไม่รองรับ ARCore กรุณาทดลองกับมือถือที่รองรับ'**
  String get arAvailabilityUnsupported;

  /// No description provided for @arAvailabilityUnknown.
  ///
  /// In th, this message translates to:
  /// **'ตรวจสอบการรองรับ AR ไม่ได้'**
  String get arAvailabilityUnknown;

  /// No description provided for @arStart.
  ///
  /// In th, this message translates to:
  /// **'เริ่มวัดพื้น'**
  String get arStart;

  /// No description provided for @arResultTitle.
  ///
  /// In th, this message translates to:
  /// **'ผลการวัดล่าสุด'**
  String get arResultTitle;

  /// No description provided for @arResultArea.
  ///
  /// In th, this message translates to:
  /// **'พื้นที่'**
  String get arResultArea;

  /// No description provided for @arResultPerimeter.
  ///
  /// In th, this message translates to:
  /// **'เส้นรอบรูป'**
  String get arResultPerimeter;

  /// No description provided for @arResultEdge.
  ///
  /// In th, this message translates to:
  /// **'ด้านที่ {index}'**
  String arResultEdge(int index);

  /// No description provided for @arValueMeters.
  ///
  /// In th, this message translates to:
  /// **'{value} ม.'**
  String arValueMeters(String value);

  /// No description provided for @arValueSquareMeters.
  ///
  /// In th, this message translates to:
  /// **'{value} ตร.ม.'**
  String arValueSquareMeters(String value);

  /// No description provided for @arUnitMeter.
  ///
  /// In th, this message translates to:
  /// **'ม.'**
  String get arUnitMeter;

  /// No description provided for @arUnitSquareMeter.
  ///
  /// In th, this message translates to:
  /// **'ตร.ม.'**
  String get arUnitSquareMeter;

  /// No description provided for @arSearchingPlane.
  ///
  /// In th, this message translates to:
  /// **'ส่องกล้องไปที่พื้นแล้วค่อยๆ ขยับเครื่อง'**
  String get arSearchingPlane;

  /// No description provided for @arTapToPlace.
  ///
  /// In th, this message translates to:
  /// **'แตะบนพื้นเพื่อวางจุดแรก'**
  String get arTapToPlace;

  /// No description provided for @arAddMorePoints.
  ///
  /// In th, this message translates to:
  /// **'แตะวางจุดถัดไป ต้องมีอย่างน้อย 3 จุด'**
  String get arAddMorePoints;

  /// No description provided for @arReadyToClose.
  ///
  /// In th, this message translates to:
  /// **'วางครบทุกมุมแล้วแตะ ปิดรูป'**
  String get arReadyToClose;

  /// No description provided for @arClosedHint.
  ///
  /// In th, this message translates to:
  /// **'ตรวจรูปแล้วแตะ เสร็จ'**
  String get arClosedHint;

  /// No description provided for @arTrackingLost.
  ///
  /// In th, this message translates to:
  /// **'ติดตามภาพไม่ได้ ขยับเครื่องช้าลงและส่องพื้นที่มีลวดลาย'**
  String get arTrackingLost;

  /// No description provided for @arNotOnPlane.
  ///
  /// In th, this message translates to:
  /// **'ไม่พบพื้นตรงจุดที่แตะ'**
  String get arNotOnPlane;

  /// No description provided for @arSelfIntersecting.
  ///
  /// In th, this message translates to:
  /// **'เส้นตัดกัน แตะ ย้อนกลับ แล้ววางจุดใหม่'**
  String get arSelfIntersecting;

  /// No description provided for @arUndo.
  ///
  /// In th, this message translates to:
  /// **'ย้อนกลับ'**
  String get arUndo;

  /// No description provided for @arCloseShape.
  ///
  /// In th, this message translates to:
  /// **'ปิดรูป'**
  String get arCloseShape;

  /// No description provided for @arDone.
  ///
  /// In th, this message translates to:
  /// **'เสร็จ'**
  String get arDone;

  /// No description provided for @arErrorCameraPermission.
  ///
  /// In th, this message translates to:
  /// **'ไม่ได้รับสิทธิ์ใช้กล้อง กรุณาอนุญาตในการตั้งค่าเครื่อง'**
  String get arErrorCameraPermission;

  /// No description provided for @arErrorInstallDeclined.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่ได้ติดตั้ง Google Play Services for AR'**
  String get arErrorInstallDeclined;

  /// No description provided for @arErrorUnavailable.
  ///
  /// In th, this message translates to:
  /// **'เครื่องนี้ใช้ AR ไม่ได้'**
  String get arErrorUnavailable;

  /// No description provided for @arErrorSessionFailed.
  ///
  /// In th, this message translates to:
  /// **'เปิดกล้อง AR ไม่สำเร็จ กรุณาลองใหม่'**
  String get arErrorSessionFailed;

  /// No description provided for @arErrorBusy.
  ///
  /// In th, this message translates to:
  /// **'กำลังวัดอยู่แล้ว'**
  String get arErrorBusy;

  /// No description provided for @photoAreaCardTitle.
  ///
  /// In th, this message translates to:
  /// **'วัดพื้นที่จากรูป + A4'**
  String get photoAreaCardTitle;

  /// No description provided for @photoAreaCardBody.
  ///
  /// In th, this message translates to:
  /// **'ถ่ายรูปพื้นที่ที่มีกระดาษขนาดมาตรฐานวางอยู่ แล้วแตะมุมกระดาษและมุมพื้นที่'**
  String get photoAreaCardBody;

  /// No description provided for @photoAreaReferenceLabel.
  ///
  /// In th, this message translates to:
  /// **'กระดาษอ้างอิง'**
  String get photoAreaReferenceLabel;

  /// No description provided for @photoAreaReferencePlaceholder.
  ///
  /// In th, this message translates to:
  /// **'เลือกขนาดอ้างอิง'**
  String get photoAreaReferencePlaceholder;

  /// No description provided for @photoAreaPresetA4.
  ///
  /// In th, this message translates to:
  /// **'A4 (210 × 297 มม.)'**
  String get photoAreaPresetA4;

  /// No description provided for @photoAreaPresetA3.
  ///
  /// In th, this message translates to:
  /// **'A3 (297 × 420 มม.)'**
  String get photoAreaPresetA3;

  /// No description provided for @photoAreaPresetCustom.
  ///
  /// In th, this message translates to:
  /// **'กำหนดขนาดเอง'**
  String get photoAreaPresetCustom;

  /// No description provided for @photoAreaWidthLabel.
  ///
  /// In th, this message translates to:
  /// **'กว้าง (มม.)'**
  String get photoAreaWidthLabel;

  /// No description provided for @photoAreaWidthPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'เช่น 210'**
  String get photoAreaWidthPlaceholder;

  /// No description provided for @photoAreaHeightLabel.
  ///
  /// In th, this message translates to:
  /// **'ยาว (มม.)'**
  String get photoAreaHeightLabel;

  /// No description provided for @photoAreaHeightPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'เช่น 297'**
  String get photoAreaHeightPlaceholder;

  /// No description provided for @photoAreaErrorSize.
  ///
  /// In th, this message translates to:
  /// **'กรอกความกว้างและความยาวที่มากกว่า 0'**
  String get photoAreaErrorSize;

  /// No description provided for @photoAreaPickGallery.
  ///
  /// In th, this message translates to:
  /// **'เลือกจากคลังรูป'**
  String get photoAreaPickGallery;

  /// No description provided for @photoAreaPickCamera.
  ///
  /// In th, this message translates to:
  /// **'ถ่ายรูป'**
  String get photoAreaPickCamera;

  /// No description provided for @photoAreaErrorPick.
  ///
  /// In th, this message translates to:
  /// **'เปิดรูปไม่สำเร็จ กรุณาลองใหม่'**
  String get photoAreaErrorPick;

  /// No description provided for @photoAreaTitle.
  ///
  /// In th, this message translates to:
  /// **'วัดจากรูป'**
  String get photoAreaTitle;

  /// No description provided for @photoAreaStepReference.
  ///
  /// In th, this message translates to:
  /// **'แตะมุมทั้ง 4 ของกระดาษอ้างอิง ({count}/4)'**
  String photoAreaStepReference(int count);

  /// No description provided for @photoAreaStepArea.
  ///
  /// In th, this message translates to:
  /// **'แตะมุมของพื้นที่ อย่างน้อย 3 จุด ({count})'**
  String photoAreaStepArea(int count);

  /// No description provided for @photoAreaStepAdjust.
  ///
  /// In th, this message translates to:
  /// **'ลากจุดเพื่อปรับตำแหน่ง'**
  String get photoAreaStepAdjust;

  /// No description provided for @photoAreaClear.
  ///
  /// In th, this message translates to:
  /// **'ล้าง'**
  String get photoAreaClear;

  /// No description provided for @photoAreaWidth.
  ///
  /// In th, this message translates to:
  /// **'กว้าง'**
  String get photoAreaWidth;

  /// No description provided for @photoAreaLength.
  ///
  /// In th, this message translates to:
  /// **'ยาว'**
  String get photoAreaLength;

  /// No description provided for @photoAreaVertices.
  ///
  /// In th, this message translates to:
  /// **'จำนวนจุด'**
  String get photoAreaVertices;

  /// No description provided for @photoAreaValueMm.
  ///
  /// In th, this message translates to:
  /// **'{value} มม.'**
  String photoAreaValueMm(String value);

  /// No description provided for @photoAreaErrorReference.
  ///
  /// In th, this message translates to:
  /// **'มุมกระดาษอ้างอิงไม่ถูกต้อง ย้ายจุดให้ตรงมุมทั้งสี่ของกระดาษ'**
  String get photoAreaErrorReference;

  /// No description provided for @syncBannerOffline.
  ///
  /// In th, this message translates to:
  /// **'ออฟไลน์ บันทึกลงเครื่องแล้ว'**
  String get syncBannerOffline;

  /// No description provided for @syncBannerSyncing.
  ///
  /// In th, this message translates to:
  /// **'กำลังซิงก์ข้อมูล ({count})'**
  String syncBannerSyncing(int count);

  /// No description provided for @syncBannerPending.
  ///
  /// In th, this message translates to:
  /// **'รอส่งขึ้นเซิร์ฟเวอร์ {count} รายการ'**
  String syncBannerPending(int count);

  /// No description provided for @syncBannerConflict.
  ///
  /// In th, this message translates to:
  /// **'พบข้อมูลขัดแย้ง {count} รายการ แตะเพื่อตรวจสอบ'**
  String syncBannerConflict(int count);

  /// No description provided for @syncQueueTitle.
  ///
  /// In th, this message translates to:
  /// **'คิวซิงก์ข้อมูล'**
  String get syncQueueTitle;

  /// No description provided for @syncQueueEmpty.
  ///
  /// In th, this message translates to:
  /// **'ซิงก์ข้อมูลครบทุกรายการแล้ว'**
  String get syncQueueEmpty;

  /// No description provided for @syncQueueSendNow.
  ///
  /// In th, this message translates to:
  /// **'ส่งทั้งหมดตอนนี้'**
  String get syncQueueSendNow;

  /// No description provided for @syncQueueClearFailed.
  ///
  /// In th, this message translates to:
  /// **'ลบรายการที่ค้าง'**
  String get syncQueueClearFailed;

  /// No description provided for @syncStatusPending.
  ///
  /// In th, this message translates to:
  /// **'รอส่ง'**
  String get syncStatusPending;

  /// No description provided for @syncStatusSyncing.
  ///
  /// In th, this message translates to:
  /// **'กำลังส่ง'**
  String get syncStatusSyncing;

  /// No description provided for @syncStatusConflict.
  ///
  /// In th, this message translates to:
  /// **'ข้อมูลขัดแย้ง'**
  String get syncStatusConflict;

  /// No description provided for @syncStatusFailed.
  ///
  /// In th, this message translates to:
  /// **'ส่งไม่สำเร็จ'**
  String get syncStatusFailed;

  /// No description provided for @numpadClear.
  ///
  /// In th, this message translates to:
  /// **'ล้าง'**
  String get numpadClear;

  /// No description provided for @numpadBackspace.
  ///
  /// In th, this message translates to:
  /// **'ลบ'**
  String get numpadBackspace;

  /// No description provided for @numpadDone.
  ///
  /// In th, this message translates to:
  /// **'ตกลง'**
  String get numpadDone;

  /// No description provided for @todayTitle.
  ///
  /// In th, this message translates to:
  /// **'งานวันนี้'**
  String get todayTitle;

  /// No description provided for @todayRefresh.
  ///
  /// In th, this message translates to:
  /// **'โหลดข้อมูลใหม่'**
  String get todayRefresh;

  /// No description provided for @todaySearchPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'ค้นหาเลขงาน ชื่อลูกค้า หรือสถานที่'**
  String get todaySearchPlaceholder;

  /// No description provided for @todayFilterAll.
  ///
  /// In th, this message translates to:
  /// **'ทั้งหมด'**
  String get todayFilterAll;

  /// No description provided for @todayFilterSurveying.
  ///
  /// In th, this message translates to:
  /// **'กำลังสำรวจ'**
  String get todayFilterSurveying;

  /// No description provided for @todayFilterQualified.
  ///
  /// In th, this message translates to:
  /// **'รอนัดสำรวจ'**
  String get todayFilterQualified;

  /// No description provided for @todayEmptyTitle.
  ///
  /// In th, this message translates to:
  /// **'ไม่มีงานสำรวจวันนี้'**
  String get todayEmptyTitle;

  /// No description provided for @todayEmptyBody.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่มีรายการนัดสำรวจในสาขานี้ แตะเพื่อโหลดข้อมูลใหม่'**
  String get todayEmptyBody;

  /// No description provided for @todayOpenSurvey.
  ///
  /// In th, this message translates to:
  /// **'เปิดใบสำรวจ'**
  String get todayOpenSurvey;

  /// No description provided for @todayStartSurvey.
  ///
  /// In th, this message translates to:
  /// **'เริ่มสำรวจหน้างาน'**
  String get todayStartSurvey;

  /// No description provided for @todayStageQualified.
  ///
  /// In th, this message translates to:
  /// **'ผ่านคัดกรอง'**
  String get todayStageQualified;

  /// No description provided for @todayStageSurveying.
  ///
  /// In th, this message translates to:
  /// **'กำลังสำรวจ'**
  String get todayStageSurveying;

  /// No description provided for @todayStageEstimating.
  ///
  /// In th, this message translates to:
  /// **'กำลังประเมินราคา'**
  String get todayStageEstimating;

  /// No description provided for @todayNoSite.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่ระบุสถานที่'**
  String get todayNoSite;

  /// No description provided for @todayScheduledAt.
  ///
  /// In th, this message translates to:
  /// **'นัดหมาย {date}'**
  String todayScheduledAt(String date);

  /// No description provided for @surveyListTitle.
  ///
  /// In th, this message translates to:
  /// **'สำรวจหน้างาน'**
  String get surveyListTitle;

  /// No description provided for @surveySearchPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'ค้นหาเลขใบสำรวจ หรือชื่องาน'**
  String get surveySearchPlaceholder;

  /// No description provided for @surveyEmptyTitle.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่มีใบสำรวจ'**
  String get surveyEmptyTitle;

  /// No description provided for @surveyEmptyBody.
  ///
  /// In th, this message translates to:
  /// **'เลือกงานจากหน้า งานวันนี้ เพื่อเริ่มบันทึกผลสำรวจหน้างาน'**
  String get surveyEmptyBody;

  /// No description provided for @surveyGoToToday.
  ///
  /// In th, this message translates to:
  /// **'ไปหน้างานวันนี้'**
  String get surveyGoToToday;

  /// No description provided for @surveyStatusDraft.
  ///
  /// In th, this message translates to:
  /// **'ฉบับร่าง'**
  String get surveyStatusDraft;

  /// No description provided for @surveyStatusReady.
  ///
  /// In th, this message translates to:
  /// **'พร้อมประเมินราคา'**
  String get surveyStatusReady;

  /// No description provided for @surveyStatusVoided.
  ///
  /// In th, this message translates to:
  /// **'ยกเลิก'**
  String get surveyStatusVoided;

  /// No description provided for @surveyStatusScheduled.
  ///
  /// In th, this message translates to:
  /// **'นัดหมายแล้ว'**
  String get surveyStatusScheduled;

  /// No description provided for @surveyDetailTitle.
  ///
  /// In th, this message translates to:
  /// **'ใบสำรวจ {number}'**
  String surveyDetailTitle(String number);

  /// No description provided for @surveyScopeLabel.
  ///
  /// In th, this message translates to:
  /// **'สรุปขอบเขตงาน'**
  String get surveyScopeLabel;

  /// No description provided for @surveyScopePlaceholder.
  ///
  /// In th, this message translates to:
  /// **'ระบุรายละเอียดหน้างานหรือสิ่งที่ลูกค้าต้องการ'**
  String get surveyScopePlaceholder;

  /// No description provided for @surveyAreasSection.
  ///
  /// In th, this message translates to:
  /// **'พื้นที่สำรวจ'**
  String get surveyAreasSection;

  /// No description provided for @surveySelectArea.
  ///
  /// In th, this message translates to:
  /// **'เลือกพื้นที่สำรวจ'**
  String get surveySelectArea;

  /// No description provided for @surveyBreakdownMeasurements.
  ///
  /// In th, this message translates to:
  /// **'รายการค่าวัดย่อย'**
  String get surveyBreakdownMeasurements;

  /// No description provided for @surveyTabletInspectorTitle.
  ///
  /// In th, this message translates to:
  /// **'แผงสรุปผลสำรวจและสเปก'**
  String get surveyTabletInspectorTitle;

  /// No description provided for @surveyAddArea.
  ///
  /// In th, this message translates to:
  /// **'เพิ่มพื้นที่'**
  String get surveyAddArea;

  /// No description provided for @surveyAreaNameLabel.
  ///
  /// In th, this message translates to:
  /// **'ชื่อพื้นที่'**
  String get surveyAreaNameLabel;

  /// No description provided for @surveyAreaNamePlaceholder.
  ///
  /// In th, this message translates to:
  /// **'เช่น ห้องโถงชั้น 1, ห้องครัว'**
  String get surveyAreaNamePlaceholder;

  /// No description provided for @surveyAreaCodeLabel.
  ///
  /// In th, this message translates to:
  /// **'รหัสพื้นที่'**
  String get surveyAreaCodeLabel;

  /// No description provided for @surveyAreaCodePlaceholder.
  ///
  /// In th, this message translates to:
  /// **'เช่น A-01'**
  String get surveyAreaCodePlaceholder;

  /// No description provided for @surveyAreaNotesLabel.
  ///
  /// In th, this message translates to:
  /// **'หมายเหตุพื้นที่'**
  String get surveyAreaNotesLabel;

  /// No description provided for @surveyAreaNotesPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'ข้อจำกัดหน้างานหรือรายละเอียดเพิ่มเติม'**
  String get surveyAreaNotesPlaceholder;

  /// No description provided for @surveyDeleteArea.
  ///
  /// In th, this message translates to:
  /// **'ลบพื้นที่'**
  String get surveyDeleteArea;

  /// No description provided for @surveyDeleteAreaConfirm.
  ///
  /// In th, this message translates to:
  /// **'ต้องการลบพื้นที่นี้และค่าวัดทั้งหมดในพื้นที่ใช่หรือไม่'**
  String get surveyDeleteAreaConfirm;

  /// No description provided for @surveyMeasurementsSection.
  ///
  /// In th, this message translates to:
  /// **'รายการวัดขนาด'**
  String get surveyMeasurementsSection;

  /// No description provided for @surveyAddMeasurement.
  ///
  /// In th, this message translates to:
  /// **'เพิ่มค่าวัด'**
  String get surveyAddMeasurement;

  /// No description provided for @surveyMeasureTypeArea.
  ///
  /// In th, this message translates to:
  /// **'พื้นที่'**
  String get surveyMeasureTypeArea;

  /// No description provided for @surveyMeasureTypeLength.
  ///
  /// In th, this message translates to:
  /// **'ความยาว'**
  String get surveyMeasureTypeLength;

  /// No description provided for @surveyMeasureTypeWidth.
  ///
  /// In th, this message translates to:
  /// **'ความกว้าง'**
  String get surveyMeasureTypeWidth;

  /// No description provided for @surveyMeasureTypeHeight.
  ///
  /// In th, this message translates to:
  /// **'ความสูง'**
  String get surveyMeasureTypeHeight;

  /// No description provided for @surveyMeasureTypeCount.
  ///
  /// In th, this message translates to:
  /// **'จำนวน'**
  String get surveyMeasureTypeCount;

  /// No description provided for @surveyUnitSqm.
  ///
  /// In th, this message translates to:
  /// **'ตร.ม.'**
  String get surveyUnitSqm;

  /// No description provided for @surveyUnitM.
  ///
  /// In th, this message translates to:
  /// **'ม.'**
  String get surveyUnitM;

  /// No description provided for @surveyUnitMm.
  ///
  /// In th, this message translates to:
  /// **'มม.'**
  String get surveyUnitMm;

  /// No description provided for @surveyUnitPcs.
  ///
  /// In th, this message translates to:
  /// **'ชิ้น'**
  String get surveyUnitPcs;

  /// No description provided for @surveyCaptureMeasured.
  ///
  /// In th, this message translates to:
  /// **'วัดจริง'**
  String get surveyCaptureMeasured;

  /// No description provided for @surveyCapturePhoto.
  ///
  /// In th, this message translates to:
  /// **'วัดจากรูป A4'**
  String get surveyCapturePhoto;

  /// No description provided for @surveyCaptureAr.
  ///
  /// In th, this message translates to:
  /// **'วัดด้วย AR'**
  String get surveyCaptureAr;

  /// No description provided for @surveyCaptureCustomer.
  ///
  /// In th, this message translates to:
  /// **'ลูกค้าแจ้ง'**
  String get surveyCaptureCustomer;

  /// No description provided for @surveyMeasureByPad.
  ///
  /// In th, this message translates to:
  /// **'ป้อนตัวเลข'**
  String get surveyMeasureByPad;

  /// No description provided for @surveyMeasureByPhoto.
  ///
  /// In th, this message translates to:
  /// **'วัดจากรูป A4'**
  String get surveyMeasureByPhoto;

  /// No description provided for @surveyMeasureByAr.
  ///
  /// In th, this message translates to:
  /// **'AR วัดพื้น'**
  String get surveyMeasureByAr;

  /// No description provided for @surveyMeasureNotesLabel.
  ///
  /// In th, this message translates to:
  /// **'หมายเหตุค่าวัด'**
  String get surveyMeasureNotesLabel;

  /// No description provided for @surveyMeasureNotesPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'เช่น หักเสา 2 ต้น หรือวัดรวมบัว'**
  String get surveyMeasureNotesPlaceholder;

  /// No description provided for @surveySaveDraft.
  ///
  /// In th, this message translates to:
  /// **'บันทึกร่าง'**
  String get surveySaveDraft;

  /// No description provided for @surveySavedOffline.
  ///
  /// In th, this message translates to:
  /// **'บันทึกลงคิวออฟไลน์แล้ว ระบบจะซิงก์เมื่อออนไลน์'**
  String get surveySavedOffline;

  /// No description provided for @surveySavedOnline.
  ///
  /// In th, this message translates to:
  /// **'บันทึกใบสำรวจสำเร็จ'**
  String get surveySavedOnline;

  /// No description provided for @surveyMarkReady.
  ///
  /// In th, this message translates to:
  /// **'ยืนยันพร้อมประเมินราคา'**
  String get surveyMarkReady;

  /// No description provided for @surveyMarkReadyConfirm.
  ///
  /// In th, this message translates to:
  /// **'เมื่อยืนยันแล้วจะไม่สามารถแก้ไขฉบับนี้ได้ ต้องการยืนยันใช่หรือไม่'**
  String get surveyMarkReadyConfirm;

  /// No description provided for @surveyNoAreas.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่มีพื้นที่สำรวจ แตะ เพิ่มพื้นที่ เพื่อเริ่มวัด'**
  String get surveyNoAreas;

  /// No description provided for @scanTitle.
  ///
  /// In th, this message translates to:
  /// **'สแกนสินค้า'**
  String get scanTitle;

  /// No description provided for @scanSearchLabel.
  ///
  /// In th, this message translates to:
  /// **'รหัสบาร์โค้ด หรือรหัสสินค้า'**
  String get scanSearchLabel;

  /// No description provided for @scanSearchPlaceholder.
  ///
  /// In th, this message translates to:
  /// **'กรอกบาร์โค้ด หรือรหัส SKU เช่น ITM-0001'**
  String get scanSearchPlaceholder;

  /// No description provided for @scanSearchAction.
  ///
  /// In th, this message translates to:
  /// **'ค้นหา'**
  String get scanSearchAction;

  /// No description provided for @scanToggleCamera.
  ///
  /// In th, this message translates to:
  /// **'เปิดกล้องสแกน'**
  String get scanToggleCamera;

  /// No description provided for @scanCloseCamera.
  ///
  /// In th, this message translates to:
  /// **'ปิดกล้อง'**
  String get scanCloseCamera;

  /// No description provided for @scanCameraHint.
  ///
  /// In th, this message translates to:
  /// **'เล็งกรอบให้ตรงกับบาร์โค้ดหรือ QR ของสินค้า'**
  String get scanCameraHint;

  /// No description provided for @scanResultTitle.
  ///
  /// In th, this message translates to:
  /// **'ข้อมูลสินค้า'**
  String get scanResultTitle;

  /// No description provided for @scanItemCode.
  ///
  /// In th, this message translates to:
  /// **'รหัสสินค้า'**
  String get scanItemCode;

  /// No description provided for @scanItemType.
  ///
  /// In th, this message translates to:
  /// **'ประเภท'**
  String get scanItemType;

  /// No description provided for @scanBaseUnit.
  ///
  /// In th, this message translates to:
  /// **'หน่วยนับหลัก'**
  String get scanBaseUnit;

  /// No description provided for @scanBarcodeValue.
  ///
  /// In th, this message translates to:
  /// **'บาร์โค้ด'**
  String get scanBarcodeValue;

  /// No description provided for @scanPackaging.
  ///
  /// In th, this message translates to:
  /// **'หน่วยบรรจุ'**
  String get scanPackaging;

  /// No description provided for @scanStockSection.
  ///
  /// In th, this message translates to:
  /// **'ยอดสต็อกคงเหลือตามคลัง'**
  String get scanStockSection;

  /// No description provided for @scanStockAvailable.
  ///
  /// In th, this message translates to:
  /// **'พร้อมใช้ {qty} {unit}'**
  String scanStockAvailable(String qty, String unit);

  /// No description provided for @scanStockOnHand.
  ///
  /// In th, this message translates to:
  /// **'คงคลัง {onHand} จอง {reserved}'**
  String scanStockOnHand(String onHand, String reserved);

  /// No description provided for @scanStockEmpty.
  ///
  /// In th, this message translates to:
  /// **'ไม่พบยอดคงเหลือในคลังของสาขานี้'**
  String get scanStockEmpty;

  /// No description provided for @scanRecentTitle.
  ///
  /// In th, this message translates to:
  /// **'สแกนล่าสุด'**
  String get scanRecentTitle;

  /// No description provided for @scanEmptyTitle.
  ///
  /// In th, this message translates to:
  /// **'พร้อมสแกนหรือค้นหาสินค้า'**
  String get scanEmptyTitle;

  /// No description provided for @scanEmptyBody.
  ///
  /// In th, this message translates to:
  /// **'สแกนบาร์โค้ดด้วยกล้อง หรือพิมพ์รหัสสินค้าด้านบนเพื่อดูรายละเอียดและยอดสต็อก'**
  String get scanEmptyBody;

  /// No description provided for @scanNotFound.
  ///
  /// In th, this message translates to:
  /// **'ไม่พบสินค้าที่ตรงกับรหัสนี้'**
  String get scanNotFound;

  /// No description provided for @notificationsTitle.
  ///
  /// In th, this message translates to:
  /// **'แจ้งเตือน'**
  String get notificationsTitle;

  /// No description provided for @notificationsFilterAll.
  ///
  /// In th, this message translates to:
  /// **'ทั้งหมด'**
  String get notificationsFilterAll;

  /// No description provided for @notificationsFilterUnread.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่อ่าน'**
  String get notificationsFilterUnread;

  /// No description provided for @notificationsMarkAllRead.
  ///
  /// In th, this message translates to:
  /// **'อ่านทั้งหมด'**
  String get notificationsMarkAllRead;

  /// No description provided for @notificationsMarkRead.
  ///
  /// In th, this message translates to:
  /// **'ทำเครื่องหมายว่าอ่านแล้ว'**
  String get notificationsMarkRead;

  /// No description provided for @notificationsEmptyTitle.
  ///
  /// In th, this message translates to:
  /// **'ไม่มีรายการแจ้งเตือน'**
  String get notificationsEmptyTitle;

  /// No description provided for @notificationsEmptyBody.
  ///
  /// In th, this message translates to:
  /// **'เมื่อมีงานรออนุมัติหรือการเปลี่ยนแปลงที่เกี่ยวข้อง จะแสดงที่นี่'**
  String get notificationsEmptyBody;

  /// No description provided for @notificationsUnreadBadge.
  ///
  /// In th, this message translates to:
  /// **'ใหม่'**
  String get notificationsUnreadBadge;

  /// No description provided for @notificationsReadBadge.
  ///
  /// In th, this message translates to:
  /// **'อ่านแล้ว'**
  String get notificationsReadBadge;

  /// No description provided for @notificationsTypeEstimate.
  ///
  /// In th, this message translates to:
  /// **'ใบประเมินราคา'**
  String get notificationsTypeEstimate;

  /// No description provided for @notificationsTypeCost.
  ///
  /// In th, this message translates to:
  /// **'ต้นทุนสินค้า'**
  String get notificationsTypeCost;

  /// No description provided for @notificationsTypePo.
  ///
  /// In th, this message translates to:
  /// **'ใบสั่งซื้อ'**
  String get notificationsTypePo;

  /// No description provided for @notificationsTypeChangeOrder.
  ///
  /// In th, this message translates to:
  /// **'ใบสั่งเปลี่ยนแปลงงาน'**
  String get notificationsTypeChangeOrder;

  /// No description provided for @notificationsTypeMrp.
  ///
  /// In th, this message translates to:
  /// **'แผนวัสดุ MRP'**
  String get notificationsTypeMrp;

  /// No description provided for @notificationsTypeRole.
  ///
  /// In th, this message translates to:
  /// **'สิทธิ์ผู้ใช้งาน'**
  String get notificationsTypeRole;

  /// No description provided for @notificationsTypeMembership.
  ///
  /// In th, this message translates to:
  /// **'อายุสมาชิกสาขา'**
  String get notificationsTypeMembership;

  /// No description provided for @notificationsTypeGeneral.
  ///
  /// In th, this message translates to:
  /// **'แจ้งเตือนระบบ'**
  String get notificationsTypeGeneral;

  /// No description provided for @attachmentSectionTitle.
  ///
  /// In th, this message translates to:
  /// **'รูปถ่ายและหลักฐานพื้นที่'**
  String get attachmentSectionTitle;

  /// No description provided for @attachmentTakePhoto.
  ///
  /// In th, this message translates to:
  /// **'ถ่ายรูป'**
  String get attachmentTakePhoto;

  /// No description provided for @attachmentPickGallery.
  ///
  /// In th, this message translates to:
  /// **'คลังภาพ'**
  String get attachmentPickGallery;

  /// No description provided for @attachmentEmptyHint.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่มีรูปถ่ายในพื้นที่นี้'**
  String get attachmentEmptyHint;

  /// No description provided for @attachmentStatusPending.
  ///
  /// In th, this message translates to:
  /// **'รอซิงก์'**
  String get attachmentStatusPending;

  /// No description provided for @attachmentStatusSynced.
  ///
  /// In th, this message translates to:
  /// **'ซิงก์แล้ว'**
  String get attachmentStatusSynced;

  /// No description provided for @attachmentDeleteTitle.
  ///
  /// In th, this message translates to:
  /// **'ลบรูปถ่าย'**
  String get attachmentDeleteTitle;

  /// No description provided for @attachmentDeleteConfirm.
  ///
  /// In th, this message translates to:
  /// **'ต้องการลบรูปถ่ายนี้ออกจากพื้นที่สำรวจใช่หรือไม่'**
  String get attachmentDeleteConfirm;

  /// No description provided for @surveyCaptureLaser.
  ///
  /// In th, this message translates to:
  /// **'วัดด้วยเลเซอร์ BLE'**
  String get surveyCaptureLaser;

  /// No description provided for @surveyMeasureByLaser.
  ///
  /// In th, this message translates to:
  /// **'วัดด้วยเลเซอร์ Bluetooth'**
  String get surveyMeasureByLaser;

  /// No description provided for @laserSheetTitle.
  ///
  /// In th, this message translates to:
  /// **'เครื่องวัดระยะเลเซอร์ Bluetooth'**
  String get laserSheetTitle;

  /// No description provided for @laserScanStart.
  ///
  /// In th, this message translates to:
  /// **'สแกนหาเครื่องวัดเลเซอร์'**
  String get laserScanStart;

  /// No description provided for @laserScanStop.
  ///
  /// In th, this message translates to:
  /// **'หยุดสแกน'**
  String get laserScanStop;

  /// No description provided for @laserScanning.
  ///
  /// In th, this message translates to:
  /// **'กำลังค้นหาอุปกรณ์ BLE...'**
  String get laserScanning;

  /// No description provided for @laserNoDevices.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่พบเครื่องวัดเลเซอร์ในระยะใกล้'**
  String get laserNoDevices;

  /// No description provided for @laserConnect.
  ///
  /// In th, this message translates to:
  /// **'เชื่อมต่อ'**
  String get laserConnect;

  /// No description provided for @laserDisconnect.
  ///
  /// In th, this message translates to:
  /// **'ตัดการเชื่อมต่อ'**
  String get laserDisconnect;

  /// No description provided for @laserConnectedLabel.
  ///
  /// In th, this message translates to:
  /// **'เชื่อมต่อแล้ว: {deviceName}'**
  String laserConnectedLabel(String deviceName);

  /// No description provided for @laserLatestReading.
  ///
  /// In th, this message translates to:
  /// **'ค่าวัดล่าสุดจากเลเซอร์'**
  String get laserLatestReading;

  /// No description provided for @laserUseReading.
  ///
  /// In th, this message translates to:
  /// **'ใช้ค่าวัดนี้'**
  String get laserUseReading;

  /// No description provided for @laserSimulateButton.
  ///
  /// In th, this message translates to:
  /// **'จำลองค่าวัดเลเซอร์ (โหมดทดสอบ)'**
  String get laserSimulateButton;

  /// No description provided for @laserSimulatedDeviceName.
  ///
  /// In th, this message translates to:
  /// **'DISTO-SIM (จำลอง BLE)'**
  String get laserSimulatedDeviceName;

  /// No description provided for @laserBluetoothOff.
  ///
  /// In th, this message translates to:
  /// **'บลูทูธปิดอยู่ หรืออุปกรณ์ไม่รองรับ BLE สามารถใช้ปุ่มจำลองค่าวัดเพื่อทดสอบได้'**
  String get laserBluetoothOff;

  /// No description provided for @pushSectionTitle.
  ///
  /// In th, this message translates to:
  /// **'การแจ้งเตือนแบบพุช (Push Notification)'**
  String get pushSectionTitle;

  /// No description provided for @pushStatusEnabled.
  ///
  /// In th, this message translates to:
  /// **'เปิดรับแจ้งเตือนแล้ว'**
  String get pushStatusEnabled;

  /// No description provided for @pushStatusDisabled.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่ได้เปิดรับแจ้งเตือน'**
  String get pushStatusDisabled;

  /// No description provided for @pushStatusUnavailable.
  ///
  /// In th, this message translates to:
  /// **'ยังไม่ได้ตั้งค่า Firebase Push บนเครื่องนี้'**
  String get pushStatusUnavailable;

  /// No description provided for @pushEnableAction.
  ///
  /// In th, this message translates to:
  /// **'ขอสิทธิ์รับแจ้งเตือนพุช'**
  String get pushEnableAction;

  /// No description provided for @pushTokenLabel.
  ///
  /// In th, this message translates to:
  /// **'รหัสอุปกรณ์ (FCM Token)'**
  String get pushTokenLabel;

  /// No description provided for @quickEstimateOpenButton.
  ///
  /// In th, this message translates to:
  /// **'ประเมินราคาด่วนหน้างาน'**
  String get quickEstimateOpenButton;

  /// No description provided for @quickEstimateTitle.
  ///
  /// In th, this message translates to:
  /// **'ประเมินราคาด่วน ({number})'**
  String quickEstimateTitle(String number);

  /// No description provided for @quickEstimateCreateAction.
  ///
  /// In th, this message translates to:
  /// **'สร้างใบประเมินราคาด่วน'**
  String get quickEstimateCreateAction;

  /// No description provided for @quickEstimateTemplateLabel.
  ///
  /// In th, this message translates to:
  /// **'แม่แบบราคา'**
  String get quickEstimateTemplateLabel;

  /// No description provided for @quickEstimatePropertyTypeLabel.
  ///
  /// In th, this message translates to:
  /// **'ประเภทอสังหาริมทรัพย์'**
  String get quickEstimatePropertyTypeLabel;

  /// No description provided for @quickEstimatePropertyCondo.
  ///
  /// In th, this message translates to:
  /// **'คอนโดมิเนียม'**
  String get quickEstimatePropertyCondo;

  /// No description provided for @quickEstimatePropertyHouse.
  ///
  /// In th, this message translates to:
  /// **'บ้านเดี่ยว / ทาวน์โฮม'**
  String get quickEstimatePropertyHouse;

  /// No description provided for @quickEstimatePropertyOffice.
  ///
  /// In th, this message translates to:
  /// **'สำนักงาน / พื้นที่พาณิชย์'**
  String get quickEstimatePropertyOffice;

  /// No description provided for @quickEstimateGradeLabel.
  ///
  /// In th, this message translates to:
  /// **'เกรดวัสดุ'**
  String get quickEstimateGradeLabel;

  /// No description provided for @quickEstimateGradeStandard.
  ///
  /// In th, this message translates to:
  /// **'มาตรฐาน (Standard)'**
  String get quickEstimateGradeStandard;

  /// No description provided for @quickEstimateGradePremium.
  ///
  /// In th, this message translates to:
  /// **'พรีเมียม (Premium)'**
  String get quickEstimateGradePremium;

  /// No description provided for @quickEstimateGradeLuxury.
  ///
  /// In th, this message translates to:
  /// **'ลักชัวรี (Luxury)'**
  String get quickEstimateGradeLuxury;

  /// No description provided for @quickEstimateCalculateAction.
  ///
  /// In th, this message translates to:
  /// **'คำนวณช่วงราคา'**
  String get quickEstimateCalculateAction;

  /// No description provided for @quickEstimateRangeLabel.
  ///
  /// In th, this message translates to:
  /// **'ช่วงราคาประเมินเบื้องต้น (รวมภาษี)'**
  String get quickEstimateRangeLabel;

  /// No description provided for @quickEstimateNetAmountLabel.
  ///
  /// In th, this message translates to:
  /// **'ยอดก่อนภาษี'**
  String get quickEstimateNetAmountLabel;

  /// No description provided for @quickEstimateTaxAmountLabel.
  ///
  /// In th, this message translates to:
  /// **'ภาษีมูลค่าเพิ่ม 7%'**
  String get quickEstimateTaxAmountLabel;

  /// No description provided for @quickEstimateValidUntilLabel.
  ///
  /// In th, this message translates to:
  /// **'ยืนราคาถึงวันที่ {date}'**
  String quickEstimateValidUntilLabel(String date);

  /// No description provided for @quickEstimateShareAction.
  ///
  /// In th, this message translates to:
  /// **'แสดงหน้าจอแชร์ให้ลูกค้า'**
  String get quickEstimateShareAction;

  /// No description provided for @quickEstimateShareSheetTitle.
  ///
  /// In th, this message translates to:
  /// **'ข้อเสนอราคาเบื้องต้นสำหรับลูกค้า'**
  String get quickEstimateShareSheetTitle;

  /// No description provided for @quickEstimateShareDisclaimer.
  ///
  /// In th, this message translates to:
  /// **'ราคานี้เป็นการประเมินเบื้องต้นจากพื้นที่สำรวจหน้างาน ราคาทางการจะออกหลังตรวจสอบรายการวัสดุ (BOQ)'**
  String get quickEstimateShareDisclaimer;

  /// No description provided for @quickEstimateShareConfirmAction.
  ///
  /// In th, this message translates to:
  /// **'บันทึกการแชร์ให้ลูกค้า'**
  String get quickEstimateShareConfirmAction;

  /// No description provided for @quickEstimateShareSuccess.
  ///
  /// In th, this message translates to:
  /// **'บันทึกการแชร์ช่วงราคาเรียบร้อยแล้ว'**
  String get quickEstimateShareSuccess;

  /// No description provided for @quickEstimateConvertAction.
  ///
  /// In th, this message translates to:
  /// **'แปลงเป็นร่างใบประเมินทางการ (BOQ)'**
  String get quickEstimateConvertAction;

  /// No description provided for @quickEstimateConvertConfirmTitle.
  ///
  /// In th, this message translates to:
  /// **'ยืนยันแปลงเป็นร่างใบประเมินทางการ?'**
  String get quickEstimateConvertConfirmTitle;

  /// No description provided for @quickEstimateConvertConfirmBody.
  ///
  /// In th, this message translates to:
  /// **'ระบบจะสร้างร่างใบประเมินราคาทางการ (Official Estimate Draft) จากใบสำรวจหน้างานและรายการวัดนี้'**
  String get quickEstimateConvertConfirmBody;

  /// No description provided for @quickEstimateConvertSuccess.
  ///
  /// In th, this message translates to:
  /// **'สร้างร่างใบประเมินราคาทางการเรียบร้อยแล้ว'**
  String get quickEstimateConvertSuccess;

  /// No description provided for @quickEstimateLinesSection.
  ///
  /// In th, this message translates to:
  /// **'รายการพื้นที่และปริมาณที่นำมาคำนวณ'**
  String get quickEstimateLinesSection;

  /// No description provided for @approvalSheetTitle.
  ///
  /// In th, this message translates to:
  /// **'พิจารณาอนุมัติเอกสาร'**
  String get approvalSheetTitle;

  /// No description provided for @approvalDocumentLabel.
  ///
  /// In th, this message translates to:
  /// **'เลขที่เอกสาร'**
  String get approvalDocumentLabel;

  /// No description provided for @approvalSummaryLabel.
  ///
  /// In th, this message translates to:
  /// **'รายการ'**
  String get approvalSummaryLabel;

  /// No description provided for @approvalNoteLabel.
  ///
  /// In th, this message translates to:
  /// **'หมายเหตุประกอบการพิจารณา'**
  String get approvalNoteLabel;

  /// No description provided for @approvalNotePlaceholder.
  ///
  /// In th, this message translates to:
  /// **'ระบุเหตุผลหรือข้อสังเกต (จำเป็นเมื่อส่งกลับแก้ไข)...'**
  String get approvalNotePlaceholder;

  /// No description provided for @approvalApproveAction.
  ///
  /// In th, this message translates to:
  /// **'อนุมัติเอกสาร'**
  String get approvalApproveAction;

  /// No description provided for @approvalReturnAction.
  ///
  /// In th, this message translates to:
  /// **'ส่งกลับแก้ไข / ปฏิเสธ'**
  String get approvalReturnAction;

  /// No description provided for @approvalConfirmApproveTitle.
  ///
  /// In th, this message translates to:
  /// **'ยืนยันการอนุมัติเอกสาร?'**
  String get approvalConfirmApproveTitle;

  /// No description provided for @approvalConfirmReturnTitle.
  ///
  /// In th, this message translates to:
  /// **'ยืนยันการส่งกลับ / ปฏิเสธเอกสาร?'**
  String get approvalConfirmReturnTitle;

  /// No description provided for @approvalSuccessApproved.
  ///
  /// In th, this message translates to:
  /// **'บันทึกการอนุมัติเอกสารเรียบร้อยแล้ว'**
  String get approvalSuccessApproved;

  /// No description provided for @approvalSuccessReturned.
  ///
  /// In th, this message translates to:
  /// **'บันทึกการส่งกลับแก้ไขเรียบร้อยแล้ว'**
  String get approvalSuccessReturned;

  /// No description provided for @approvalNoteRequiredError.
  ///
  /// In th, this message translates to:
  /// **'กรุณาระบุหมายเหตุก่อนส่งกลับแก้ไข'**
  String get approvalNoteRequiredError;

  /// No description provided for @approvalQuickActionHint.
  ///
  /// In th, this message translates to:
  /// **'แตะเพื่อพิจารณาอนุมัติด่วน'**
  String get approvalQuickActionHint;

  /// No description provided for @conflictSheetTitle.
  ///
  /// In th, this message translates to:
  /// **'เปรียบเทียบข้อมูลขัดแย้ง (409/412)'**
  String get conflictSheetTitle;

  /// No description provided for @conflictDescription.
  ///
  /// In th, this message translates to:
  /// **'ข้อมูลบนเซิร์ฟเวอร์ถูกแก้ไขโดยผู้ใช้อื่นในระหว่างที่คุณทำงานออฟไลน์ กรุณาเลือกเวอร์ชันที่ต้องการใช้'**
  String get conflictDescription;

  /// No description provided for @conflictFieldColumn.
  ///
  /// In th, this message translates to:
  /// **'ฟิลด์ข้อมูล'**
  String get conflictFieldColumn;

  /// No description provided for @conflictLocalColumn.
  ///
  /// In th, this message translates to:
  /// **'ค่าในเครื่องของคุณ'**
  String get conflictLocalColumn;

  /// No description provided for @conflictServerColumn.
  ///
  /// In th, this message translates to:
  /// **'ค่าบนเซิร์ฟเวอร์'**
  String get conflictServerColumn;

  /// No description provided for @conflictKeepLocalAction.
  ///
  /// In th, this message translates to:
  /// **'ใช้ค่าในเครื่อง (ส่งซ้ำ)'**
  String get conflictKeepLocalAction;

  /// No description provided for @conflictAcceptServerAction.
  ///
  /// In th, this message translates to:
  /// **'ใช้ค่าบนเซิร์ฟเวอร์ (ละทิ้งของเครื่อง)'**
  String get conflictAcceptServerAction;

  /// No description provided for @conflictResolveAction.
  ///
  /// In th, this message translates to:
  /// **'ตรวจสอบและแก้ไขข้อขัดแย้ง'**
  String get conflictResolveAction;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['en', 'th'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en':
      return AppLocalizationsEn();
    case 'th':
      return AppLocalizationsTh();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
