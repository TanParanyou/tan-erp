import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_measure_display.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/features/device_lab/data/ar_measure_repository.dart';
import 'package:tan_erp_field/features/device_lab/domain/ar_measure_failure.dart';
import 'package:tan_erp_field/features/device_lab/presentation/ar_measure_controller.dart';
import 'package:tan_erp_field/features/device_lab/presentation/photo_area_card.dart';
import 'package:tan_erp_field/native/laser_meter/laser_meter_service.dart';

/// Dev-flavor only: tries device-specific features. Nothing is saved.
class DeviceLabPage extends StatelessWidget {
  const DeviceLabPage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    return ErpScreen(
      title: l10n.deviceLabTitle,
      body: ListView(
        padding: ErpPagePadding.of(context),
        children: [
          Text(
            l10n.deviceLabIntro,
            style: Theme.of(context).textTheme.bodyMedium,
          ),
          SizedBox(height: t.space4),
          const _ArMeasureCard(),
          SizedBox(height: t.space4),
          const PhotoAreaCard(),
          SizedBox(height: t.space4),
          const LaserMeterLabCard(),
        ],
      ),
    );
  }
}

class _ArMeasureCard extends ConsumerWidget {
  const _ArMeasureCard();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final availability = ref.watch(arAvailabilityProvider);
    final measure = ref.watch(arMeasureControllerProvider);
    // Unknown may still work: ARCore's install flow gives the final answer.
    final canStart = switch (availability.value) {
      ArAvailability.supported ||
      ArAvailability.needsInstall ||
      ArAvailability.unknown => true,
      ArAvailability.unsupported || null => false,
    };

    return ErpCard(
      title: l10n.arCardTitle,
      children: [
        Text(l10n.arCardBody, style: text.bodyMedium),
        SizedBox(height: t.space4),
        availability.when(
          loading: () => ErpMonoLoading(size: t.space6),
          error: (_, _) => ErpAlert(
            message: l10n.arAvailabilityUnknown,
            severity: ErpAlertSeverity.warning,
          ),
          data: (a) => ErpAlert(
            message: switch (a) {
              ArAvailability.supported => l10n.arAvailabilitySupported,
              ArAvailability.needsInstall => l10n.arAvailabilityNeedsInstall,
              ArAvailability.unsupported => l10n.arAvailabilityUnsupported,
              ArAvailability.unknown => l10n.arAvailabilityUnknown,
            },
            severity: switch (a) {
              ArAvailability.supported => ErpAlertSeverity.success,
              ArAvailability.needsInstall => ErpAlertSeverity.info,
              ArAvailability.unsupported ||
              ArAvailability.unknown => ErpAlertSeverity.warning,
            },
          ),
        ),
        if (measure.failure != null) ...[
          SizedBox(height: t.space3),
          ErpAlert(
            message: _failureMessage(l10n, measure.failure!),
            severity: ErpAlertSeverity.danger,
          ),
        ],
        SizedBox(height: t.space4),
        ErpButton(
          label: l10n.arStart,
          isLoading: measure.isMeasuring,
          onPressed: canStart
              ? () => ref
                    .read(arMeasureControllerProvider.notifier)
                    .measure(_request(context))
              : null,
        ),
        if (measure.result != null) ...[
          SizedBox(height: t.space4),
          _ResultCard(result: measure.result!),
        ],
      ],
    );
  }

  static String _failureMessage(AppLocalizations l10n, ArMeasureFailure f) =>
      switch (f) {
        ArMeasureFailure.cameraPermissionDenied => l10n.arErrorCameraPermission,
        ArMeasureFailure.installDeclined => l10n.arErrorInstallDeclined,
        ArMeasureFailure.unavailable => l10n.arErrorUnavailable,
        ArMeasureFailure.busy => l10n.arErrorBusy,
        ArMeasureFailure.sessionFailed ||
        ArMeasureFailure.unknown => l10n.arErrorSessionFailed,
      };

  /// Native screen gets every text and colour from here (l10n + ErpTokens).
  static ArMeasureRequest _request(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    return ArMeasureRequest(
      strings: ArMeasureStrings(
        localeTag: Localizations.localeOf(context).toLanguageTag(),
        searchingPlane: l10n.arSearchingPlane,
        tapToPlace: l10n.arTapToPlace,
        addMorePoints: l10n.arAddMorePoints,
        readyToClose: l10n.arReadyToClose,
        closedHint: l10n.arClosedHint,
        trackingLost: l10n.arTrackingLost,
        notOnPlane: l10n.arNotOnPlane,
        selfIntersecting: l10n.arSelfIntersecting,
        areaLabel: l10n.arResultArea,
        unitMeter: l10n.arUnitMeter,
        unitSquareMeter: l10n.arUnitSquareMeter,
        undo: l10n.arUndo,
        closeShape: l10n.arCloseShape,
        done: l10n.arDone,
        cancel: l10n.commonCancel,
      ),
      colors: ArMeasureColors(
        surface: t.surface.toARGB32(),
        textMain: t.textMain.toARGB32(),
        textMuted: t.textMuted.toARGB32(),
        action: t.action.toARGB32(),
        onAction: t.onAction.toARGB32(),
        accentMeasure: t.accentMeasure.toARGB32(),
        danger: t.danger.toARGB32(),
        borderStrong: t.borderStrong.toARGB32(),
        // Same disabled pair as ErpButton.
        disabledBackground: t.surfaceSubtle.toARGB32(),
        disabledForeground: t.textSubtle.toARGB32(),
      ),
    );
  }
}

class _ResultCard extends StatelessWidget {
  const _ResultCard({required this.result});

  final ArMeasureResult result;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final number = NumberFormat(
      '#,##0.00',
      Localizations.localeOf(context).toLanguageTag(),
    );

    Widget row(String label, String value) => Padding(
      padding: EdgeInsets.only(top: t.space2),
      child: Row(
        children: [
          Expanded(child: Text(label, style: text.bodyMedium)),
          Text(value, style: text.bodyLarge),
        ],
      ),
    );

    return ErpCard(
      children: [
        Text(l10n.arResultTitle, style: text.titleSmall),
        SizedBox(height: t.space3),
        ErpMeasureDisplay(
          label: l10n.arResultArea,
          valueText: l10n.arValueSquareMeters(number.format(result.areaSqm)),
        ),
        SizedBox(height: t.space2),
        row(
          l10n.arResultPerimeter,
          l10n.arValueMeters(number.format(result.perimeterM)),
        ),
        for (final (i, edge) in result.edgeLengthsM.indexed)
          row(
            l10n.arResultEdge(i + 1),
            l10n.arValueMeters(number.format(edge)),
          ),
      ],
    );
  }
}

/// Device Lab card: connects or simulates a BLE laser meter.
class LaserMeterLabCard extends StatefulWidget {
  const LaserMeterLabCard({super.key});

  @override
  State<LaserMeterLabCard> createState() => _LaserMeterLabCardState();
}

class _LaserMeterLabCardState extends State<LaserMeterLabCard> {
  double? _lastReadingMeters;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final fmt = NumberFormat(
      '#,##0.00',
      Localizations.localeOf(context).toLanguageTag(),
    );

    return ErpCard(
      title: l10n.laserSheetTitle,
      children: [
        if (_lastReadingMeters != null) ...[
          ErpMeasureDisplay(
            label: l10n.laserLatestReading,
            valueText:
                '${fmt.format(_lastReadingMeters)} ${l10n.surveyUnitM}',
          ),
          SizedBox(height: t.space4),
        ],
        ErpButton(
          label: l10n.laserScanStart,
          onPressed: () async {
            final meters = await showLaserMeterSheet(context);
            if (meters != null && mounted) {
              setState(() => _lastReadingMeters = meters);
            }
          },
        ),
      ],
    );
  }
}
