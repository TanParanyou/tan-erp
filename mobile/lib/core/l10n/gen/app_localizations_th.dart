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
  String get galleryCardTitle => 'หัวการ์ด';

  @override
  String get galleryCardBody => 'เนื้อหาในการ์ด';

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

  @override
  String get syncBannerOffline => 'ออฟไลน์ บันทึกลงเครื่องแล้ว';

  @override
  String syncBannerSyncing(int count) {
    return 'กำลังซิงก์ข้อมูล ($count)';
  }

  @override
  String syncBannerPending(int count) {
    return 'รอส่งขึ้นเซิร์ฟเวอร์ $count รายการ';
  }

  @override
  String syncBannerConflict(int count) {
    return 'พบข้อมูลขัดแย้ง $count รายการ แตะเพื่อตรวจสอบ';
  }

  @override
  String get syncQueueTitle => 'คิวซิงก์ข้อมูล';

  @override
  String get syncQueueEmpty => 'ซิงก์ข้อมูลครบทุกรายการแล้ว';

  @override
  String get syncQueueSendNow => 'ส่งทั้งหมดตอนนี้';

  @override
  String get syncQueueClearFailed => 'ลบรายการที่ค้าง';

  @override
  String get syncStatusPending => 'รอส่ง';

  @override
  String get syncStatusSyncing => 'กำลังส่ง';

  @override
  String get syncStatusConflict => 'ข้อมูลขัดแย้ง';

  @override
  String get syncStatusFailed => 'ส่งไม่สำเร็จ';

  @override
  String get numpadClear => 'ล้าง';

  @override
  String get numpadBackspace => 'ลบ';

  @override
  String get numpadDone => 'ตกลง';

  @override
  String get todayTitle => 'งานวันนี้';

  @override
  String get todayRefresh => 'โหลดข้อมูลใหม่';

  @override
  String get todaySearchPlaceholder => 'ค้นหาเลขงาน ชื่อลูกค้า หรือสถานที่';

  @override
  String get todayFilterAll => 'ทั้งหมด';

  @override
  String get todayFilterSurveying => 'กำลังสำรวจ';

  @override
  String get todayFilterQualified => 'รอนัดสำรวจ';

  @override
  String get todayEmptyTitle => 'ไม่มีงานสำรวจวันนี้';

  @override
  String get todayEmptyBody =>
      'ยังไม่มีรายการนัดสำรวจในสาขานี้ แตะเพื่อโหลดข้อมูลใหม่';

  @override
  String get todayOpenSurvey => 'เปิดใบสำรวจ';

  @override
  String get todayStartSurvey => 'เริ่มสำรวจหน้างาน';

  @override
  String get todayStageQualified => 'ผ่านคัดกรอง';

  @override
  String get todayStageSurveying => 'กำลังสำรวจ';

  @override
  String get todayStageEstimating => 'กำลังประเมินราคา';

  @override
  String get todayNoSite => 'ยังไม่ระบุสถานที่';

  @override
  String todayScheduledAt(String date) {
    return 'นัดหมาย $date';
  }

  @override
  String get surveyListTitle => 'สำรวจหน้างาน';

  @override
  String get surveySearchPlaceholder => 'ค้นหาเลขใบสำรวจ หรือชื่องาน';

  @override
  String get surveyEmptyTitle => 'ยังไม่มีใบสำรวจ';

  @override
  String get surveyEmptyBody =>
      'เลือกงานจากหน้า งานวันนี้ เพื่อเริ่มบันทึกผลสำรวจหน้างาน';

  @override
  String get surveyGoToToday => 'ไปหน้างานวันนี้';

  @override
  String get surveyStatusDraft => 'ฉบับร่าง';

  @override
  String get surveyStatusReady => 'พร้อมประเมินราคา';

  @override
  String get surveyStatusVoided => 'ยกเลิก';

  @override
  String get surveyStatusScheduled => 'นัดหมายแล้ว';

  @override
  String surveyDetailTitle(String number) {
    return 'ใบสำรวจ $number';
  }

  @override
  String get surveyScopeLabel => 'สรุปขอบเขตงาน';

  @override
  String get surveyScopePlaceholder =>
      'ระบุรายละเอียดหน้างานหรือสิ่งที่ลูกค้าต้องการ';

  @override
  String get surveyAreasSection => 'พื้นที่สำรวจ';

  @override
  String get surveySelectArea => 'เลือกพื้นที่สำรวจ';

  @override
  String get surveyBreakdownMeasurements => 'รายการค่าวัดย่อย';

  @override
  String get surveyTabletInspectorTitle => 'แผงสรุปผลสำรวจและสเปก';

  @override
  String get surveyAddArea => 'เพิ่มพื้นที่';

  @override
  String get surveyAreaNameLabel => 'ชื่อพื้นที่';

  @override
  String get surveyAreaNamePlaceholder => 'เช่น ห้องโถงชั้น 1, ห้องครัว';

  @override
  String get surveyAreaCodeLabel => 'รหัสพื้นที่';

  @override
  String get surveyAreaCodePlaceholder => 'เช่น A-01';

  @override
  String get surveyAreaNotesLabel => 'หมายเหตุพื้นที่';

  @override
  String get surveyAreaNotesPlaceholder =>
      'ข้อจำกัดหน้างานหรือรายละเอียดเพิ่มเติม';

  @override
  String get surveyDeleteArea => 'ลบพื้นที่';

  @override
  String get surveyDeleteAreaConfirm =>
      'ต้องการลบพื้นที่นี้และค่าวัดทั้งหมดในพื้นที่ใช่หรือไม่';

  @override
  String get surveyMeasurementsSection => 'รายการวัดขนาด';

  @override
  String get surveyAddMeasurement => 'เพิ่มค่าวัด';

  @override
  String get surveyMeasureTypeArea => 'พื้นที่';

  @override
  String get surveyMeasureTypeLength => 'ความยาว';

  @override
  String get surveyMeasureTypeWidth => 'ความกว้าง';

  @override
  String get surveyMeasureTypeHeight => 'ความสูง';

  @override
  String get surveyMeasureTypeCount => 'จำนวน';

  @override
  String get surveyUnitSqm => 'ตร.ม.';

  @override
  String get surveyUnitM => 'ม.';

  @override
  String get surveyUnitMm => 'มม.';

  @override
  String get surveyUnitPcs => 'ชิ้น';

  @override
  String get surveyCaptureMeasured => 'วัดจริง';

  @override
  String get surveyCapturePhoto => 'วัดจากรูป A4';

  @override
  String get surveyCaptureAr => 'วัดด้วย AR';

  @override
  String get surveyCaptureCustomer => 'ลูกค้าแจ้ง';

  @override
  String get surveyMeasureByPad => 'ป้อนตัวเลข';

  @override
  String get surveyMeasureByPhoto => 'วัดจากรูป A4';

  @override
  String get surveyMeasureByAr => 'AR วัดพื้น';

  @override
  String get surveyMeasureNotesLabel => 'หมายเหตุค่าวัด';

  @override
  String get surveyMeasureNotesPlaceholder => 'เช่น หักเสา 2 ต้น หรือวัดรวมบัว';

  @override
  String get surveySaveDraft => 'บันทึกร่าง';

  @override
  String get surveySavedOffline =>
      'บันทึกลงคิวออฟไลน์แล้ว ระบบจะซิงก์เมื่อออนไลน์';

  @override
  String get surveySavedOnline => 'บันทึกใบสำรวจสำเร็จ';

  @override
  String get surveyMarkReady => 'ยืนยันพร้อมประเมินราคา';

  @override
  String get surveyMarkReadyConfirm =>
      'เมื่อยืนยันแล้วจะไม่สามารถแก้ไขฉบับนี้ได้ ต้องการยืนยันใช่หรือไม่';

  @override
  String get surveyNoAreas =>
      'ยังไม่มีพื้นที่สำรวจ แตะ เพิ่มพื้นที่ เพื่อเริ่มวัด';

  @override
  String get scanTitle => 'สแกนสินค้า';

  @override
  String get scanSearchLabel => 'รหัสบาร์โค้ด หรือรหัสสินค้า';

  @override
  String get scanSearchPlaceholder => 'กรอกบาร์โค้ด หรือรหัส SKU เช่น ITM-0001';

  @override
  String get scanSearchAction => 'ค้นหา';

  @override
  String get scanToggleCamera => 'เปิดกล้องสแกน';

  @override
  String get scanCloseCamera => 'ปิดกล้อง';

  @override
  String get scanCameraHint => 'เล็งกรอบให้ตรงกับบาร์โค้ดหรือ QR ของสินค้า';

  @override
  String get scanResultTitle => 'ข้อมูลสินค้า';

  @override
  String get scanItemCode => 'รหัสสินค้า';

  @override
  String get scanItemType => 'ประเภท';

  @override
  String get scanBaseUnit => 'หน่วยนับหลัก';

  @override
  String get scanBarcodeValue => 'บาร์โค้ด';

  @override
  String get scanPackaging => 'หน่วยบรรจุ';

  @override
  String get scanStockSection => 'ยอดสต็อกคงเหลือตามคลัง';

  @override
  String scanStockAvailable(String qty, String unit) {
    return 'พร้อมใช้ $qty $unit';
  }

  @override
  String scanStockOnHand(String onHand, String reserved) {
    return 'คงคลัง $onHand จอง $reserved';
  }

  @override
  String get scanStockEmpty => 'ไม่พบยอดคงเหลือในคลังของสาขานี้';

  @override
  String get scanRecentTitle => 'สแกนล่าสุด';

  @override
  String get scanEmptyTitle => 'พร้อมสแกนหรือค้นหาสินค้า';

  @override
  String get scanEmptyBody =>
      'สแกนบาร์โค้ดด้วยกล้อง หรือพิมพ์รหัสสินค้าด้านบนเพื่อดูรายละเอียดและยอดสต็อก';

  @override
  String get scanNotFound => 'ไม่พบสินค้าที่ตรงกับรหัสนี้';

  @override
  String get notificationsTitle => 'แจ้งเตือน';

  @override
  String get notificationsFilterAll => 'ทั้งหมด';

  @override
  String get notificationsFilterUnread => 'ยังไม่อ่าน';

  @override
  String get notificationsMarkAllRead => 'อ่านทั้งหมด';

  @override
  String get notificationsMarkRead => 'ทำเครื่องหมายว่าอ่านแล้ว';

  @override
  String get notificationsEmptyTitle => 'ไม่มีรายการแจ้งเตือน';

  @override
  String get notificationsEmptyBody =>
      'เมื่อมีงานรออนุมัติหรือการเปลี่ยนแปลงที่เกี่ยวข้อง จะแสดงที่นี่';

  @override
  String get notificationsUnreadBadge => 'ใหม่';

  @override
  String get notificationsReadBadge => 'อ่านแล้ว';

  @override
  String get notificationsTypeEstimate => 'ใบประเมินราคา';

  @override
  String get notificationsTypeCost => 'ต้นทุนสินค้า';

  @override
  String get notificationsTypePo => 'ใบสั่งซื้อ';

  @override
  String get notificationsTypeChangeOrder => 'ใบสั่งเปลี่ยนแปลงงาน';

  @override
  String get notificationsTypeMrp => 'แผนวัสดุ MRP';

  @override
  String get notificationsTypeRole => 'สิทธิ์ผู้ใช้งาน';

  @override
  String get notificationsTypeMembership => 'อายุสมาชิกสาขา';

  @override
  String get notificationsTypeGeneral => 'แจ้งเตือนระบบ';

  @override
  String get attachmentSectionTitle => 'รูปถ่ายและหลักฐานพื้นที่';

  @override
  String get attachmentTakePhoto => 'ถ่ายรูป';

  @override
  String get attachmentPickGallery => 'คลังภาพ';

  @override
  String get attachmentEmptyHint => 'ยังไม่มีรูปถ่ายในพื้นที่นี้';

  @override
  String get attachmentStatusPending => 'รอซิงก์';

  @override
  String get attachmentStatusSynced => 'ซิงก์แล้ว';

  @override
  String get attachmentDeleteTitle => 'ลบรูปถ่าย';

  @override
  String get attachmentDeleteConfirm =>
      'ต้องการลบรูปถ่ายนี้ออกจากพื้นที่สำรวจใช่หรือไม่';

  @override
  String get surveyCaptureLaser => 'วัดด้วยเลเซอร์ BLE';

  @override
  String get surveyMeasureByLaser => 'วัดด้วยเลเซอร์ Bluetooth';

  @override
  String get laserSheetTitle => 'เครื่องวัดระยะเลเซอร์ Bluetooth';

  @override
  String get laserScanStart => 'สแกนหาเครื่องวัดเลเซอร์';

  @override
  String get laserScanStop => 'หยุดสแกน';

  @override
  String get laserScanning => 'กำลังค้นหาอุปกรณ์ BLE...';

  @override
  String get laserNoDevices => 'ยังไม่พบเครื่องวัดเลเซอร์ในระยะใกล้';

  @override
  String get laserConnect => 'เชื่อมต่อ';

  @override
  String get laserDisconnect => 'ตัดการเชื่อมต่อ';

  @override
  String laserConnectedLabel(String deviceName) {
    return 'เชื่อมต่อแล้ว: $deviceName';
  }

  @override
  String get laserLatestReading => 'ค่าวัดล่าสุดจากเลเซอร์';

  @override
  String get laserUseReading => 'ใช้ค่าวัดนี้';

  @override
  String get laserSimulateButton => 'จำลองค่าวัดเลเซอร์ (โหมดทดสอบ)';

  @override
  String get laserSimulatedDeviceName => 'DISTO-SIM (จำลอง BLE)';

  @override
  String get laserBluetoothOff =>
      'บลูทูธปิดอยู่ หรืออุปกรณ์ไม่รองรับ BLE สามารถใช้ปุ่มจำลองค่าวัดเพื่อทดสอบได้';

  @override
  String get pushSectionTitle => 'การแจ้งเตือนแบบพุช (Push Notification)';

  @override
  String get pushStatusEnabled => 'เปิดรับแจ้งเตือนแล้ว';

  @override
  String get pushStatusDisabled => 'ยังไม่ได้เปิดรับแจ้งเตือน';

  @override
  String get pushStatusUnavailable =>
      'ยังไม่ได้ตั้งค่า Firebase Push บนเครื่องนี้';

  @override
  String get pushEnableAction => 'ขอสิทธิ์รับแจ้งเตือนพุช';

  @override
  String get pushTokenLabel => 'รหัสอุปกรณ์ (FCM Token)';

  @override
  String get quickEstimateOpenButton => 'ประเมินราคาด่วนหน้างาน';

  @override
  String quickEstimateTitle(String number) {
    return 'ประเมินราคาด่วน ($number)';
  }

  @override
  String get quickEstimateCreateAction => 'สร้างใบประเมินราคาด่วน';

  @override
  String get quickEstimateTemplateLabel => 'แม่แบบราคา';

  @override
  String get quickEstimatePropertyTypeLabel => 'ประเภทอสังหาริมทรัพย์';

  @override
  String get quickEstimatePropertyCondo => 'คอนโดมิเนียม';

  @override
  String get quickEstimatePropertyHouse => 'บ้านเดี่ยว / ทาวน์โฮม';

  @override
  String get quickEstimatePropertyOffice => 'สำนักงาน / พื้นที่พาณิชย์';

  @override
  String get quickEstimateGradeLabel => 'เกรดวัสดุ';

  @override
  String get quickEstimateGradeStandard => 'มาตรฐาน (Standard)';

  @override
  String get quickEstimateGradePremium => 'พรีเมียม (Premium)';

  @override
  String get quickEstimateGradeLuxury => 'ลักชัวรี (Luxury)';

  @override
  String get quickEstimateCalculateAction => 'คำนวณช่วงราคา';

  @override
  String get quickEstimateRangeLabel => 'ช่วงราคาประเมินเบื้องต้น (รวมภาษี)';

  @override
  String get quickEstimateNetAmountLabel => 'ยอดก่อนภาษี';

  @override
  String get quickEstimateTaxAmountLabel => 'ภาษีมูลค่าเพิ่ม 7%';

  @override
  String quickEstimateValidUntilLabel(String date) {
    return 'ยืนราคาถึงวันที่ $date';
  }

  @override
  String get quickEstimateShareAction => 'แสดงหน้าจอแชร์ให้ลูกค้า';

  @override
  String get quickEstimateShareSheetTitle => 'ข้อเสนอราคาเบื้องต้นสำหรับลูกค้า';

  @override
  String get quickEstimateShareDisclaimer =>
      'ราคานี้เป็นการประเมินเบื้องต้นจากพื้นที่สำรวจหน้างาน ราคาทางการจะออกหลังตรวจสอบรายการวัสดุ (BOQ)';

  @override
  String get quickEstimateShareConfirmAction => 'บันทึกการแชร์ให้ลูกค้า';

  @override
  String get quickEstimateShareSuccess => 'บันทึกการแชร์ช่วงราคาเรียบร้อยแล้ว';

  @override
  String get quickEstimateConvertAction => 'แปลงเป็นร่างใบประเมินทางการ (BOQ)';

  @override
  String get quickEstimateConvertConfirmTitle =>
      'ยืนยันแปลงเป็นร่างใบประเมินทางการ?';

  @override
  String get quickEstimateConvertConfirmBody =>
      'ระบบจะสร้างร่างใบประเมินราคาทางการ (Official Estimate Draft) จากใบสำรวจหน้างานและรายการวัดนี้';

  @override
  String get quickEstimateConvertSuccess =>
      'สร้างร่างใบประเมินราคาทางการเรียบร้อยแล้ว';

  @override
  String get quickEstimateLinesSection => 'รายการพื้นที่และปริมาณที่นำมาคำนวณ';

  @override
  String get approvalSheetTitle => 'พิจารณาอนุมัติเอกสาร';

  @override
  String get approvalDocumentLabel => 'เลขที่เอกสาร';

  @override
  String get approvalSummaryLabel => 'รายการ';

  @override
  String get approvalNoteLabel => 'หมายเหตุประกอบการพิจารณา';

  @override
  String get approvalNotePlaceholder =>
      'ระบุเหตุผลหรือข้อสังเกต (จำเป็นเมื่อส่งกลับแก้ไข)...';

  @override
  String get approvalApproveAction => 'อนุมัติเอกสาร';

  @override
  String get approvalReturnAction => 'ส่งกลับแก้ไข / ปฏิเสธ';

  @override
  String get approvalConfirmApproveTitle => 'ยืนยันการอนุมัติเอกสาร?';

  @override
  String get approvalConfirmReturnTitle => 'ยืนยันการส่งกลับ / ปฏิเสธเอกสาร?';

  @override
  String get approvalSuccessApproved => 'บันทึกการอนุมัติเอกสารเรียบร้อยแล้ว';

  @override
  String get approvalSuccessReturned => 'บันทึกการส่งกลับแก้ไขเรียบร้อยแล้ว';

  @override
  String get approvalNoteRequiredError => 'กรุณาระบุหมายเหตุก่อนส่งกลับแก้ไข';

  @override
  String get approvalQuickActionHint => 'แตะเพื่อพิจารณาอนุมัติด่วน';

  @override
  String get conflictSheetTitle => 'เปรียบเทียบข้อมูลขัดแย้ง (409/412)';

  @override
  String get conflictDescription =>
      'ข้อมูลบนเซิร์ฟเวอร์ถูกแก้ไขโดยผู้ใช้อื่นในระหว่างที่คุณทำงานออฟไลน์ กรุณาเลือกเวอร์ชันที่ต้องการใช้';

  @override
  String get conflictFieldColumn => 'ฟิลด์ข้อมูล';

  @override
  String get conflictLocalColumn => 'ค่าในเครื่องของคุณ';

  @override
  String get conflictServerColumn => 'ค่าบนเซิร์ฟเวอร์';

  @override
  String get conflictKeepLocalAction => 'ใช้ค่าในเครื่อง (ส่งซ้ำ)';

  @override
  String get conflictAcceptServerAction =>
      'ใช้ค่าบนเซิร์ฟเวอร์ (ละทิ้งของเครื่อง)';

  @override
  String get conflictResolveAction => 'ตรวจสอบและแก้ไขข้อขัดแย้ง';
}
