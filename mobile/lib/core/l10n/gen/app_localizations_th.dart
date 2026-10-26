// ignore: unused_import
import 'package:intl/intl.dart' as intl;

import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Thai (`th`).
class AppLocalizationsTh extends AppLocalizations {
  AppLocalizationsTh([String locale = 'th']) : super(locale);

  @override
  String get appTitle => 'TAN Field';

  @override
  String get commonCancel => 'ยกเลิก';

  @override
  String get commonConfirm => 'ยืนยัน';

  @override
  String get commonRetry => 'ลองใหม่';

  @override
  String get commonSelect => 'เลือก';

  @override
  String get commonFieldRequired => 'กรุณากรอกข้อมูล';

  @override
  String get loginTitle => 'เข้าสู่ระบบ';

  @override
  String get loginEmailLabel => 'อีเมล';

  @override
  String get loginEmailPlaceholder => 'name@company.com';

  @override
  String get loginPasswordLabel => 'รหัสผ่าน';

  @override
  String get loginPasswordPlaceholder => 'กรอกรหัสผ่าน';

  @override
  String get loginSubmit => 'เข้าสู่ระบบ';

  @override
  String get notConfiguredTitle => 'ยังไม่ได้ตั้งค่า Firebase';

  @override
  String get notConfiguredBody =>
      'เพิ่มค่า FIREBASE_* ในไฟล์ config ของ flavor นี้ แล้วรันแอปใหม่ด้วย --dart-define-from-file';

  @override
  String get unlockTitle => 'ปลดล็อกแอป';

  @override
  String get unlockBody => 'ยืนยันตัวตนเพื่อเข้าใช้งานต่อ';

  @override
  String get unlockAction => 'ปลดล็อก';

  @override
  String get unlockReason => 'ยืนยันตัวตนเพื่อเข้าใช้ TAN Field';

  @override
  String get unlockUseOtherAccount => 'ใช้บัญชีอื่น';

  @override
  String get membershipTitle => 'เลือกสาขา';

  @override
  String get membershipLabel => 'สาขาที่ทำงาน';

  @override
  String get membershipAllBranches => 'ทุกสาขา';

  @override
  String get membershipContinue => 'เข้าใช้งาน';

  @override
  String get navToday => 'งานวันนี้';

  @override
  String get navSurvey => 'สำรวจ';

  @override
  String get navScan => 'สแกน';

  @override
  String get navNotifications => 'แจ้งเตือน';

  @override
  String get navMe => 'ฉัน';

  @override
  String get inDevelopmentTitle => 'กำลังพัฒนา';

  @override
  String get inDevelopmentBody => 'เมนูนี้จะเปิดให้ใช้งานในเวอร์ชันถัดไป';

  @override
  String get meTitle => 'ฉัน';

  @override
  String get meProfile => 'โปรไฟล์';

  @override
  String get meOrganization => 'องค์กร';

  @override
  String get meBranch => 'สาขา';

  @override
  String get meSwitchBranch => 'เปลี่ยนสาขา';

  @override
  String get meLanguage => 'ภาษา';

  @override
  String get meAppVersion => 'เวอร์ชันแอป';

  @override
  String get meLogout => 'ออกจากระบบ';

  @override
  String get meUiGallery => 'ตัวอย่างคอมโพเนนต์';

  @override
  String get languageThai => 'ไทย';

  @override
  String get languageEnglish => 'English';

  @override
  String get themeMode => 'โหมดสี';

  @override
  String get themeModeSystem => 'ตามเครื่อง';

  @override
  String get themeModeLight => 'สว่าง';

  @override
  String get themeModeDark => 'มืด';

  @override
  String get themeDensity => 'ความแน่น';

  @override
  String get themeDensityNormal => 'ปกติ';

  @override
  String get themeDensityCompact => 'กะทัดรัด';

  @override
  String get themeTextScale => 'ขนาดตัวอักษร';

  @override
  String themeTextScaleValue(int percent) {
    return '$percent%';
  }

  @override
  String get logoutConfirmTitle => 'ออกจากระบบ';

  @override
  String get logoutConfirmBody => 'ต้องการออกจากระบบใช่หรือไม่';

  @override
  String get forceUpdateTitle => 'ต้องอัปเดตแอป';

  @override
  String forceUpdateBody(String minimum) {
    return 'เวอร์ชันนี้ไม่รองรับแล้ว กรุณาอัปเดตเป็นเวอร์ชัน $minimum ขึ้นไป';
  }

  @override
  String forceUpdateCurrent(String current) {
    return 'เวอร์ชันปัจจุบัน $current';
  }

  @override
  String get errorInvalidCredentials => 'อีเมลหรือรหัสผ่านไม่ถูกต้อง';

  @override
  String get errorAccountDisabled => 'บัญชีนี้ถูกปิดการใช้งาน';

  @override
  String get errorNoMembership => 'บัญชีนี้ยังไม่มีสิทธิ์เข้าใช้งานสาขาใด';

  @override
  String get errorNetwork =>
      'เชื่อมต่อเครือข่ายไม่ได้ กรุณาตรวจสอบอินเทอร์เน็ต';

  @override
  String get errorTooManyAttempts =>
      'ลองเข้าสู่ระบบหลายครั้งเกินไป กรุณารอสักครู่';

  @override
  String get errorUnauthorized => 'เซสชันหมดอายุ กรุณาเข้าสู่ระบบใหม่';

  @override
  String get errorForbidden => 'ไม่มีสิทธิ์ทำรายการนี้';

  @override
  String get errorNotFound => 'ไม่พบข้อมูล';

  @override
  String get errorConflict => 'ข้อมูลถูกแก้ไขแล้ว กรุณาโหลดใหม่';

  @override
  String get errorValidation => 'ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง';

  @override
  String get errorBiometricFailed => 'ยืนยันตัวตนไม่สำเร็จ';

  @override
  String get errorUnknown => 'เกิดข้อผิดพลาด กรุณาลองใหม่';

  @override
  String get galleryTitle => 'ตัวอย่างคอมโพเนนต์';

  @override
  String get galleryPrimaryButton => 'ปุ่มหลัก';

  @override
  String get galleryOutlineButton => 'ปุ่มรอง';

  @override
  String get galleryGhostButton => 'ปุ่มข้อความ';

  @override
  String get galleryLoadingButton => 'กำลังบันทึก';

  @override
  String get galleryTextField => 'ช่องกรอกข้อมูล';

  @override
  String get galleryTextFieldPlaceholder => 'พิมพ์ข้อความ';

  @override
  String get gallerySelect => 'ตัวเลือก';

  @override
  String get galleryAlertInfo => 'ข้อมูลทั่วไป';

  @override
  String get galleryAlertSuccess => 'บันทึกสำเร็จ';

  @override
  String get galleryAlertWarning => 'โปรดตรวจสอบก่อนดำเนินการ';

  @override
  String get galleryAlertDanger => 'เกิดข้อผิดพลาด';

  @override
  String get galleryConfirmOpen => 'เปิดกล่องยืนยัน';

  @override
  String get galleryConfirmMessage => 'ต้องการดำเนินการต่อใช่หรือไม่';

  @override
  String get galleryEmptyTitle => 'ไม่มีรายการ';

  @override
  String get galleryEmptyMessage => 'ยังไม่มีข้อมูลในหมวดนี้';

  @override
  String get galleryEmptyAction => 'เพิ่มรายการ';
}
