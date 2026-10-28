import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_status_badge.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';
import 'package:tan_erp_field/features/scan/domain/scan_models.dart';
import 'package:tan_erp_field/features/scan/presentation/scan_controller.dart';

class ScanPage extends ConsumerStatefulWidget {
  const ScanPage({super.key});

  @override
  ConsumerState<ScanPage> createState() => _ScanPageState();
}

class _ScanPageState extends ConsumerState<ScanPage> {
  final FormControl<String> _searchControl = FormControl<String>(value: '');
  final MobileScannerController _scannerController = MobileScannerController();
  String? _lastScannedRaw;

  @override
  void dispose() {
    _searchControl.dispose();
    unawaited(_scannerController.dispose());
    super.dispose();
  }

  void _submitManualSearch() {
    final q = (_searchControl.value ?? '').trim();
    if (q.isEmpty) return;
    unawaited(ref.read(scanControllerProvider.notifier).lookup(q));
  }

  void _onBarcodeDetected(BarcodeCapture capture) {
    for (final barcode in capture.barcodes) {
      final raw = barcode.rawValue?.trim();
      if (raw != null && raw.isNotEmpty && raw != _lastScannedRaw) {
        _lastScannedRaw = raw;
        _searchControl.value = raw;
        final notifier = ref.read(scanControllerProvider.notifier)
          ..closeCamera();
        unawaited(notifier.lookup(raw));
        break;
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final l10n = context.l10n;
    final state = ref.watch(scanControllerProvider);
    final notifier = ref.read(scanControllerProvider.notifier);
    final lang = Localizations.localeOf(context).languageCode;

    return Scaffold(
      appBar: AppBar(
        title: Text(l10n.scanTitle),
        actions: [
          IconButton(
            tooltip: state.cameraOpen
                ? l10n.scanCloseCamera
                : l10n.scanToggleCamera,
            onPressed: () {
              _lastScannedRaw = null;
              notifier.toggleCamera();
            },
            icon: ErpIcon(
              ErpIcons.camera,
              color: state.cameraOpen ? t.action : t.textMain,
            ),
          ),
        ],
      ),
      body: ListView(
        padding: EdgeInsets.all(t.space4),
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Expanded(
                child: ErpTextField(
                  control: _searchControl,
                  label: l10n.scanSearchLabel,
                  placeholder: l10n.scanSearchPlaceholder,
                  textInputAction: TextInputAction.search,
                ),
              ),
              SizedBox(width: t.space2),
              SizedBox(
                width: t.tapTarget,
                height: t.tapTarget,
                child: Material(
                  color: t.action,
                  child: InkWell(
                    onTap: state.isLoading ? null : _submitManualSearch,
                    child: Center(
                      child: ErpIcon(ErpIcons.search, color: t.onAction),
                    ),
                  ),
                ),
              ),
            ],
          ),
          SizedBox(height: t.space3),
          ErpButton(
            label: state.cameraOpen
                ? l10n.scanCloseCamera
                : l10n.scanToggleCamera,
            variant: state.cameraOpen
                ? ErpButtonVariant.outline
                : ErpButtonVariant.primary,
            onPressed: () {
              _lastScannedRaw = null;
              notifier.toggleCamera();
            },
          ),
          if (state.cameraOpen) ...[
            SizedBox(height: t.space3),
            Container(
              height: 240,
              decoration: BoxDecoration(
                color: t.surface,
                border: Border.all(color: t.action, width: t.borderWidth),
              ),
              child: Stack(
                fit: StackFit.expand,
                children: [
                  MobileScanner(
                    controller: _scannerController,
                    onDetect: _onBarcodeDetected,
                    errorBuilder: (context, error) {
                      return Center(
                        child: Padding(
                          padding: EdgeInsets.all(t.space4),
                          child: Text(
                            l10n.scanCameraHint,
                            textAlign: TextAlign.center,
                            style: Theme.of(
                              context,
                            ).textTheme.bodyMedium?.copyWith(color: t.textBody),
                          ),
                        ),
                      );
                    },
                  ),
                  Align(
                    alignment: Alignment.bottomCenter,
                    child: Container(
                      width: double.infinity,
                      color: t.surfaceSubtle,
                      padding: EdgeInsets.symmetric(
                        horizontal: t.space3,
                        vertical: t.space2,
                      ),
                      child: Text(
                        l10n.scanCameraHint,
                        textAlign: TextAlign.center,
                        style: Theme.of(
                          context,
                        ).textTheme.labelSmall?.copyWith(color: t.textBody),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
          SizedBox(height: t.space4),
          if (state.isLoading)
            Padding(
              padding: EdgeInsets.symmetric(vertical: t.space6),
              child: Center(
                child: SizedBox(
                  width: 24,
                  height: 24,
                  child: CircularProgressIndicator(
                    strokeWidth: 2,
                    color: t.action,
                  ),
                ),
              ),
            )
          else if (state.notFound)
            ErpCard(
              children: [
                Text(
                  l10n.scanNotFound,
                  style: Theme.of(
                    context,
                  ).textTheme.bodyMedium?.copyWith(color: t.danger),
                ),
              ],
            )
          else if (state.currentResult != null)
            _ScannedItemDetailCard(
              result: state.currentResult!,
              languageCode: lang,
            )
          else if (state.recentScans.isEmpty)
            ErpCard(
              children: [
                Text(
                  l10n.scanEmptyTitle,
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                SizedBox(height: t.space1),
                Text(
                  l10n.scanEmptyBody,
                  style: Theme.of(
                    context,
                  ).textTheme.bodySmall?.copyWith(color: t.textBody),
                ),
              ],
            ),
          if (state.recentScans.isNotEmpty) ...[
            SizedBox(height: t.space6),
            Text(
              l10n.scanRecentTitle.toUpperCase(),
              style: Theme.of(context).textTheme.labelMedium?.copyWith(
                color: t.textBody,
                letterSpacing: 0.6,
              ),
            ),
            SizedBox(height: t.space2),
            for (final item in state.recentScans) ...[
              InkWell(
                onTap: () {
                  _searchControl.value = item.barcodeValue ?? item.code;
                  notifier.selectRecent(item);
                },
                child: ErpCard(
                  children: [
                    Row(
                      children: [
                        ErpIcon(ErpIcons.box, color: t.action),
                        SizedBox(width: t.space3),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                item.localizedName(lang),
                                style: Theme.of(context).textTheme.titleMedium,
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                              ),
                              SizedBox(height: t.space1 / 2),
                              Text(
                                item.barcodeValue != null
                                    ? '${item.code} · ${item.barcodeValue}'
                                    : item.code,
                                style: Theme.of(context).textTheme.labelSmall
                                    ?.copyWith(
                                      color: t.textMuted,
                                      fontFeatures: const [
                                        FontFeature.tabularFigures(),
                                      ],
                                    ),
                              ),
                            ],
                          ),
                        ),
                        ErpIcon(ErpIcons.chevronRight, color: t.textMuted),
                      ],
                    ),
                  ],
                ),
              ),
              SizedBox(height: t.space2),
            ],
          ],
        ],
      ),
    );
  }
}

class _ScannedItemDetailCard extends StatelessWidget {
  const _ScannedItemDetailCard({
    required this.result,
    required this.languageCode,
  });

  final ScannedItemResult result;
  final String languageCode;

  String _formatNum(double v) {
    if (v == v.roundToDouble()) {
      return v.toInt().toString();
    }
    return v.toStringAsFixed(2);
  }

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final l10n = context.l10n;
    final textTheme = Theme.of(context).textTheme;

    return ErpCard(
      children: [
        Row(
          children: [
            ErpStatusBadge(label: result.code, tone: ErpBadgeTone.info),
            const Spacer(),
            if (result.itemType.isNotEmpty)
              ErpStatusBadge(label: result.itemType),
          ],
        ),
        SizedBox(height: t.space2),
        Text(
          result.localizedName(languageCode),
          style: textTheme.titleLarge,
        ),
        SizedBox(height: t.space3),
        _MetaRow(
          label: l10n.scanItemCode,
          value: result.code,
        ),
        if (result.barcodeValue != null && result.barcodeValue!.isNotEmpty) ...[
          SizedBox(height: t.space1),
          _MetaRow(
            label: l10n.scanBarcodeValue,
            value: result.barcodeValue!,
          ),
        ],
        if (result.packagingLevel != null &&
            result.packagingLevel!.isNotEmpty) ...[
          SizedBox(height: t.space1),
          _MetaRow(
            label: l10n.scanPackaging,
            value: result.packagingLevel!,
          ),
        ],
        SizedBox(height: t.space1),
        _MetaRow(
          label: l10n.scanBaseUnit,
          value: result.baseUnitName.isNotEmpty
              ? '${result.baseUnitName} (${result.baseUnitCode})'
              : result.baseUnitCode,
        ),
        SizedBox(height: t.space4),
        Divider(height: t.borderWidth, color: t.border),
        SizedBox(height: t.space3),
        Text(
          l10n.scanStockSection.toUpperCase(),
          style: textTheme.labelMedium?.copyWith(
            color: t.textBody,
            letterSpacing: 0.6,
          ),
        ),
        SizedBox(height: t.space2),
        if (result.stocks.isEmpty)
          Text(
            l10n.scanStockEmpty,
            style: textTheme.bodySmall?.copyWith(color: t.textMuted),
          )
        else
          for (final stock in result.stocks) ...[
            Container(
              padding: EdgeInsets.all(t.space3),
              decoration: BoxDecoration(
                color: t.surfaceSubtle,
                border: Border.all(color: t.border, width: t.borderWidth),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    stock.warehouseName,
                    style: textTheme.titleMedium,
                  ),
                  SizedBox(height: t.space1),
                  Text(
                    l10n.scanStockAvailable(
                      _formatNum(stock.available),
                      result.baseUnitCode,
                    ),
                    style: textTheme.bodyMedium?.copyWith(
                      color: t.action,
                      fontWeight: FontWeight.w600,
                      fontFeatures: const [FontFeature.tabularFigures()],
                    ),
                  ),
                  SizedBox(height: t.space1 / 2),
                  Text(
                    l10n.scanStockOnHand(
                      _formatNum(stock.onHand),
                      _formatNum(stock.reserved),
                    ),
                    style: textTheme.labelSmall?.copyWith(
                      color: t.textBody,
                      fontFeatures: const [FontFeature.tabularFigures()],
                    ),
                  ),
                ],
              ),
            ),
            SizedBox(height: t.space2),
          ],
      ],
    );
  }
}

class _MetaRow extends StatelessWidget {
  const _MetaRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final textTheme = Theme.of(context).textTheme;
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          label,
          style: textTheme.bodySmall?.copyWith(color: t.textBody),
        ),
        Text(
          value,
          style: textTheme.bodyMedium?.copyWith(
            fontWeight: FontWeight.w600,
            fontFeatures: const [FontFeature.tabularFigures()],
          ),
        ),
      ],
    );
  }
}
