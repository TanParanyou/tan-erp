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
