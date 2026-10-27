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
  String get loginMockBanner => 'โหมดทดสอบ (Mock login)';

  @override
  String get loginMockUidLabel => 'รหัสผู้ใช้ (uid)';

  @override
  String get loginMockUidPlaceholder => 'เช่น foundation-user-test-only';

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

  @override
  String get deviceLabTitle => 'ห้องทดลองอุปกรณ์';

  @override
  String get deviceLabIntro =>
      'ทดลองความสามารถเฉพาะของเครื่อง ผลลัพธ์ไม่ถูกบันทึกลงระบบ';

  @override
  String get arCardTitle => 'AR วัดพื้น';

  @override
  String get arCardBody =>
      'วางจุดตามมุมห้องบนพื้นผ่านกล้อง แล้วแอปคำนวณพื้นที่ให้';

  @override
  String get arAvailabilitySupported => 'เครื่องนี้รองรับ AR';

  @override
  String get arAvailabilityNeedsInstall =>
      'ต้องติดตั้งหรืออัปเดต Google Play Services for AR ระบบจะถามเมื่อเริ่มวัด';

  @override
  String get arAvailabilityUnsupported =>
      'เครื่องนี้ไม่รองรับ ARCore กรุณาทดลองกับมือถือที่รองรับ';

  @override
  String get arAvailabilityUnknown => 'ตรวจสอบการรองรับ AR ไม่ได้';

  @override
  String get arStart => 'เริ่มวัดพื้น';

  @override
  String get arResultTitle => 'ผลการวัดล่าสุด';

  @override
  String get arResultArea => 'พื้นที่';

  @override
  String get arResultPerimeter => 'เส้นรอบรูป';

  @override
  String arResultEdge(int index) {
    return 'ด้านที่ $index';
  }

  @override
  String arValueMeters(String value) {
    return '$value ม.';
  }

  @override
  String arValueSquareMeters(String value) {
    return '$value ตร.ม.';
  }

  @override
  String get arUnitMeter => 'ม.';

  @override
  String get arUnitSquareMeter => 'ตร.ม.';

  @override
  String get arSearchingPlane => 'ส่องกล้องไปที่พื้นแล้วค่อยๆ ขยับเครื่อง';

  @override
  String get arTapToPlace => 'แตะบนพื้นเพื่อวางจุดแรก';

  @override
  String get arAddMorePoints => 'แตะวางจุดถัดไป ต้องมีอย่างน้อย 3 จุด';

  @override
  String get arReadyToClose => 'วางครบทุกมุมแล้วแตะ ปิดรูป';

  @override
  String get arClosedHint => 'ตรวจรูปแล้วแตะ เสร็จ';

  @override
  String get arTrackingLost =>
      'ติดตามภาพไม่ได้ ขยับเครื่องช้าลงและส่องพื้นที่มีลวดลาย';

  @override
  String get arNotOnPlane => 'ไม่พบพื้นตรงจุดที่แตะ';

  @override
  String get arSelfIntersecting => 'เส้นตัดกัน แตะ ย้อนกลับ แล้ววางจุดใหม่';

  @override
  String get arUndo => 'ย้อนกลับ';

  @override
  String get arCloseShape => 'ปิดรูป';

  @override
  String get arDone => 'เสร็จ';

  @override
  String get arErrorCameraPermission =>
      'ไม่ได้รับสิทธิ์ใช้กล้อง กรุณาอนุญาตในการตั้งค่าเครื่อง';

  @override
  String get arErrorInstallDeclined =>
      'ยังไม่ได้ติดตั้ง Google Play Services for AR';

  @override
  String get arErrorUnavailable => 'เครื่องนี้ใช้ AR ไม่ได้';

  @override
  String get arErrorSessionFailed => 'เปิดกล้อง AR ไม่สำเร็จ กรุณาลองใหม่';

  @override
  String get arErrorBusy => 'กำลังวัดอยู่แล้ว';

  @override
  String get photoAreaCardTitle => 'วัดพื้นที่จากรูป + A4';

  @override
  String get photoAreaCardBody =>
      'ถ่ายรูปพื้นที่ที่มีกระดาษขนาดมาตรฐานวางอยู่ แล้วแตะมุมกระดาษและมุมพื้นที่';

  @override
  String get photoAreaReferenceLabel => 'กระดาษอ้างอิง';

  @override
  String get photoAreaReferencePlaceholder => 'เลือกขนาดอ้างอิง';

  @override
  String get photoAreaPresetA4 => 'A4 (210 × 297 มม.)';

  @override
  String get photoAreaPresetA3 => 'A3 (297 × 420 มม.)';

  @override
  String get photoAreaPresetCustom => 'กำหนดขนาดเอง';

  @override
  String get photoAreaWidthLabel => 'กว้าง (มม.)';

  @override
  String get photoAreaWidthPlaceholder => 'เช่น 210';

  @override
  String get photoAreaHeightLabel => 'ยาว (มม.)';

  @override
  String get photoAreaHeightPlaceholder => 'เช่น 297';

  @override
  String get photoAreaErrorSize => 'กรอกความกว้างและความยาวที่มากกว่า 0';

  @override
  String get photoAreaPickGallery => 'เลือกจากคลังรูป';

  @override
  String get photoAreaPickCamera => 'ถ่ายรูป';

  @override
  String get photoAreaErrorPick => 'เปิดรูปไม่สำเร็จ กรุณาลองใหม่';

  @override
  String get photoAreaTitle => 'วัดจากรูป';

  @override
  String photoAreaStepReference(int count) {
    return 'แตะมุมทั้ง 4 ของกระดาษอ้างอิง ($count/4)';
  }

  @override
  String photoAreaStepArea(int count) {
    return 'แตะมุมของพื้นที่ อย่างน้อย 3 จุด ($count)';
  }

  @override
  String get photoAreaStepAdjust => 'ลากจุดเพื่อปรับตำแหน่ง';

  @override
  String get photoAreaClear => 'ล้าง';

  @override
  String get photoAreaWidth => 'กว้าง';

  @override
  String get photoAreaLength => 'ยาว';

  @override
  String get photoAreaVertices => 'จำนวนจุด';

  @override
  String photoAreaValueMm(String value) {
    return '$value มม.';
  }

  @override
  String get photoAreaErrorReference =>
      'มุมกระดาษอ้างอิงไม่ถูกต้อง ย้ายจุดให้ตรงมุมทั้งสี่ของกระดาษ';
}
