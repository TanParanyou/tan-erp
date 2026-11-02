import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/features/device_lab/domain/ar_measure_failure.dart';

/// User-facing text for an AR measure or room scan failure.
String arMeasureFailureMessage(AppLocalizations l10n, ArMeasureFailure f) =>
    switch (f) {
      ArMeasureFailure.cameraPermissionDenied => l10n.arErrorCameraPermission,
      ArMeasureFailure.installDeclined => l10n.arErrorInstallDeclined,
      ArMeasureFailure.unavailable => l10n.arErrorUnavailable,
      ArMeasureFailure.noFloor => l10n.arErrorNoFloor,
      ArMeasureFailure.busy => l10n.arErrorBusy,
      ArMeasureFailure.sessionFailed ||
      ArMeasureFailure.unknown => l10n.arErrorSessionFailed,
    };
