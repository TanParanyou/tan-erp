// Pigeon definition for the AR floor measure native module (Device Lab).
// Regenerate: dart run pigeon --input pigeons/ar_measure.dart
import 'package:pigeon/pigeon.dart';

@ConfigurePigeon(
  PigeonOptions(
    dartOut: 'lib/generated/pigeon/ar_measure_api.g.dart',
    kotlinOut:
        'android/app/src/main/kotlin/com/tanerp/tan_erp_field/ar/ArMeasureApi.g.kt',
    kotlinOptions: KotlinOptions(package: 'com.tanerp.tan_erp_field.ar'),
    swiftOut: 'ios/Runner/ArMeasureApi.g.swift',
    dartPackageName: 'tan_erp_field',
  ),
)
enum ArAvailability { supported, needsInstall, unsupported, unknown }

/// Every visible text of the native AR screen; native code has no strings.
class ArMeasureStrings {
  ArMeasureStrings({
    required this.localeTag,
    required this.searchingPlane,
    required this.tapToPlace,
    required this.addMorePoints,
    required this.readyToClose,
    required this.closedHint,
    required this.trackingLost,
    required this.notOnPlane,
    required this.selfIntersecting,
    required this.areaLabel,
    required this.unitMeter,
    required this.unitSquareMeter,
    required this.undo,
    required this.closeShape,
    required this.done,
    required this.cancel,
  });

  /// BCP 47 tag used to format numbers (e.g. `th`, `en`).
  String localeTag;
  String searchingPlane;
  String tapToPlace;
  String addMorePoints;
  String readyToClose;
  String closedHint;
  String trackingLost;
  String notOnPlane;
  String selfIntersecting;
  String areaLabel;
  String unitMeter;
  String unitSquareMeter;
  String undo;
  String closeShape;
  String done;
  String cancel;
}

/// ARGB colours taken from ErpTokens; native code has no colours.
class ArMeasureColors {
  ArMeasureColors({
    required this.surface,
    required this.textMain,
    required this.textMuted,
    required this.action,
    required this.onAction,
    required this.accentMeasure,
    required this.danger,
    required this.borderStrong,
    required this.disabledBackground,
    required this.disabledForeground,
  });

  int surface;
  int textMain;
  int textMuted;
  int action;
  int onAction;
  int accentMeasure;
  int danger;
  int borderStrong;
  int disabledBackground;
  int disabledForeground;
}

class ArMeasureRequest {
  ArMeasureRequest({required this.strings, required this.colors});

  ArMeasureStrings strings;
  ArMeasureColors colors;
}

class ArMeasureResult {
  ArMeasureResult({
    required this.areaSqm,
    required this.perimeterM,
    required this.edgeLengthsM,
    required this.vertexCount,
  });

  double areaSqm;
  double perimeterM;
  List<double> edgeLengthsM;
  int vertexCount;
}

/// Error codes of [ArMeasureApi.measureFloor] (PlatformException.code):
/// `camera_permission_denied`, `install_declined`, `unavailable`,
/// `session_failed`, `busy`.
@HostApi()
abstract class ArMeasureApi {
  @async
  ArAvailability checkAvailability();

  /// Opens the native AR screen. Returns null when the user cancels.
  @async
  ArMeasureResult? measureFloor(ArMeasureRequest request);
}

/// Texts and colours of the native room scan screen (RoomPlan draws its own
/// coaching UI; we add only the cancel/done controls).
class RoomScanRequest {
  RoomScanRequest({
    required this.cancel,
    required this.done,
    required this.colors,
  });

  String cancel;
  String done;
  ArMeasureColors colors;
}

enum RoomOpeningKind { door, window, opening }

/// One door, window or open passage found by RoomPlan.
class RoomOpening {
  RoomOpening({
    required this.kind,
    required this.widthM,
    required this.heightM,
  });

  RoomOpeningKind kind;
  double widthM;
  double heightM;
}

/// Room dimensions from a RoomPlan scan. Width/length are the floor's
/// bounding box (width ≤ length); height is the tallest wall (null when no
/// wall was detected).
class RoomScanResult {
  RoomScanResult({
    required this.floorAreaSqm,
    required this.widthM,
    required this.lengthM,
    required this.grossWallAreaSqm,
    required this.openings,
    this.ceilingHeightM,
  });

  double floorAreaSqm;
  double widthM;
  double lengthM;
  double? ceilingHeightM;

  /// Sum of every wall's width × height, openings not subtracted.
  double grossWallAreaSqm;
  List<RoomOpening> openings;
}

/// iOS only (RoomPlan, LiDAR, iOS 17+). Android registers no handler, so
/// calls fail with a channel error and the app treats it as unsupported.
/// Error codes of [RoomScanApi.scanRoom]: `camera_permission_denied`,
/// `unavailable`, `session_failed`, `no_floor`, `busy`.
@HostApi()
abstract class RoomScanApi {
  bool isSupported();

  /// Opens the native room scan. Returns null when the user cancels.
  @async
  RoomScanResult? scanRoom(RoomScanRequest request);
}
