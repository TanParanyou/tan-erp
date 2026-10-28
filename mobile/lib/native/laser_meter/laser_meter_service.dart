import 'dart:async';
import 'dart:math' as math;
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:flutter_blue_plus/flutter_blue_plus.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_measure_display.dart';

@immutable
class LaserDeviceInfo {
  const LaserDeviceInfo({
    required this.id,
    required this.name,
    required this.rssi,
  });

  final String id;
  final String name;
  final int rssi;
}

@immutable
class LaserMeterState {
  const LaserMeterState({
    this.isScanning = false,
    this.bluetoothAvailable = true,
    this.devices = const [],
    this.connectedDeviceName,
    this.latestDistanceMeters,
  });

  final bool isScanning;
  final bool bluetoothAvailable;
  final List<LaserDeviceInfo> devices;
  final String? connectedDeviceName;
  final double? latestDistanceMeters;

  LaserMeterState copyWith({
    bool? isScanning,
    bool? bluetoothAvailable,
    List<LaserDeviceInfo>? devices,
    String? Function()? connectedDeviceName,
    double? Function()? latestDistanceMeters,
  }) => LaserMeterState(
    isScanning: isScanning ?? this.isScanning,
    bluetoothAvailable: bluetoothAvailable ?? this.bluetoothAvailable,
    devices: devices ?? this.devices,
    connectedDeviceName: connectedDeviceName != null
        ? connectedDeviceName()
        : this.connectedDeviceName,
    latestDistanceMeters: latestDistanceMeters != null
        ? latestDistanceMeters()
        : this.latestDistanceMeters,
  );
}

/// BLE Laser Distance Meter controller (supports Leica DISTO / Bosch GLM GATT
/// float/ASCII distance notifications + simulator fallback for emulators).
class LaserMeterController extends Notifier<LaserMeterState> {
  StreamSubscription<List<ScanResult>>? _scanSub;
  StreamSubscription<List<int>>? _charSub;
  BluetoothDevice? _connectedDevice;
  final Map<String, BluetoothDevice> _discovered = {};
  static final _rng = math.Random();

  @override
  LaserMeterState build() {
    ref.onDispose(() {
      unawaited(_scanSub?.cancel());
      unawaited(_charSub?.cancel());
      unawaited(_connectedDevice?.disconnect());
    });
    return const LaserMeterState();
  }

  Future<void> startScan() async {
    if (state.isScanning) return;
    try {
      final supported = await FlutterBluePlus.isSupported;
      if (!supported) {
        state = state.copyWith(bluetoothAvailable: false, isScanning: false);
        return;
      }
      final adapterState = await FlutterBluePlus.adapterState.first;
      if (adapterState != BluetoothAdapterState.on) {
        state = state.copyWith(bluetoothAvailable: false, isScanning: false);
        return;
      }
      _discovered.clear();
      state = state.copyWith(
        isScanning: true,
        bluetoothAvailable: true,
        devices: const [],
      );
      await _scanSub?.cancel();
      _scanSub = FlutterBluePlus.onScanResults.listen((results) {
        final list = <LaserDeviceInfo>[];
        for (final r in results) {
          final advName = r.advertisementData.advName.trim();
          final platName = r.device.platformName.trim();
          final name = advName.isNotEmpty ? advName : platName;
          if (name.isEmpty) continue;
          final id = r.device.remoteId.str;
          _discovered[id] = r.device;
          list.add(LaserDeviceInfo(id: id, name: name, rssi: r.rssi));
        }
        state = state.copyWith(devices: list);
      });
      await FlutterBluePlus.startScan(timeout: const Duration(seconds: 6));
      state = state.copyWith(isScanning: false);
    } on Object {
      state = state.copyWith(isScanning: false, bluetoothAvailable: false);
    }
  }

  Future<void> stopScan() async {
    try {
      await FlutterBluePlus.stopScan();
    } on Object {
      // Ignore if BLE is unsupported.
    }
    state = state.copyWith(isScanning: false);
  }

  Future<void> connectDevice(LaserDeviceInfo info) async {
    final device = _discovered[info.id];
    if (device == null) return;
    try {
      await stopScan();
      await device.connect(timeout: const Duration(seconds: 8));
      _connectedDevice = device;
      state = state.copyWith(connectedDeviceName: () => info.name);

      final services = await device.discoverServices();
      for (final svc in services) {
        for (final ch in svc.characteristics) {
          if (ch.properties.notify || ch.properties.indicate) {
            await ch.setNotifyValue(true);
            await _charSub?.cancel();
            _charSub = ch.onValueReceived.listen(_parseBlePayload);
            return;
          }
        }
      }
    } on Object {
      state = state.copyWith(connectedDeviceName: () => null);
    }
  }

  Future<void> disconnect() async {
    await _charSub?.cancel();
    _charSub = null;
    try {
      await _connectedDevice?.disconnect();
    } on Object {
      // Ignore disconnect errors.
    }
    _connectedDevice = null;
    state = state.copyWith(
      connectedDeviceName: () => null,
      latestDistanceMeters: () => null,
    );
  }

  void simulateReading(String simulatedDeviceLabel) {
    final base = 1.2 + _rng.nextDouble() * 7.3;
    final rounded = double.parse(base.toStringAsFixed(2));
    state = state.copyWith(
      connectedDeviceName: () =>
          state.connectedDeviceName ?? simulatedDeviceLabel,
      latestDistanceMeters: () => rounded,
    );
  }

  void _parseBlePayload(List<int> bytes) {
    if (bytes.isEmpty) return;
    // Try IEEE-754 float32 little-endian (Leica DISTO GATT distance char).
    if (bytes.length == 4) {
      final data = ByteData.sublistView(Uint8List.fromList(bytes));
      final val = data.getFloat32(0, Endian.little);
      if (val.isFinite && val > 0.01 && val < 500) {
        state = state.copyWith(
          latestDistanceMeters: () => double.parse(val.toStringAsFixed(3)),
        );
        return;
      }
    }
    // Try ASCII numeric string.
    final ascii = String.fromCharCodes(
      bytes.where((b) => b >= 32 && b <= 126),
    ).trim();
    final match = RegExp(r'(\d+(?:\.\d+)?)').firstMatch(ascii);
    if (match != null) {
      final parsed = double.tryParse(match.group(1)!);
      if (parsed != null && parsed > 0) {
        state = state.copyWith(latestDistanceMeters: () => parsed);
      }
    }
  }
}

final laserMeterControllerProvider =
    NotifierProvider<LaserMeterController, LaserMeterState>(
      LaserMeterController.new,
    );

/// Opens a bottom sheet to scan/connect a BLE laser meter or trigger a
/// simulated reading, returning the measured distance in meters.
Future<double?> showLaserMeterSheet(BuildContext context) {
  final t = context.tokens;
  return showModalBottomSheet<double>(
    context: context,
    isScrollControlled: true,
    backgroundColor: t.surface,
    shape: RoundedRectangleBorder(borderRadius: t.corner),
    builder: (_) => const _LaserMeterSheet(),
  );
}

class _LaserMeterSheet extends ConsumerWidget {
  const _LaserMeterSheet();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final state = ref.watch(laserMeterControllerProvider);
    final notifier = ref.read(laserMeterControllerProvider.notifier);
    final fmt = NumberFormat(
      '#,##0.00',
      Localizations.localeOf(context).toLanguageTag(),
    );

    return Padding(
      padding: EdgeInsets.all(t.space4),
      child: SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(l10n.laserSheetTitle, style: text.titleMedium),
            SizedBox(height: t.space3),
            if (!state.bluetoothAvailable) ...[
              ErpAlert(message: l10n.laserBluetoothOff),
              SizedBox(height: t.space3),
            ],
            if (state.connectedDeviceName != null) ...[
              ErpAlert(
                message: l10n.laserConnectedLabel(state.connectedDeviceName!),
                severity: ErpAlertSeverity.success,
              ),
              SizedBox(height: t.space3),
            ],
            if (state.latestDistanceMeters != null) ...[
              ErpCard(
                children: [
                  ErpMeasureDisplay(
                    label: l10n.laserLatestReading,
                    valueText:
                        '${fmt.format(state.latestDistanceMeters)} '
                        '${l10n.surveyUnitM}',
                  ),
                ],
              ),
              SizedBox(height: t.space3),
              ErpButton(
                label: l10n.laserUseReading,
                onPressed: () =>
                    Navigator.of(context).pop(state.latestDistanceMeters),
              ),
              SizedBox(height: t.space2),
            ],
            Row(
              children: [
                Expanded(
                  child: ErpButton(
                    label: state.isScanning
                        ? l10n.laserScanStop
                        : l10n.laserScanStart,
                    variant: ErpButtonVariant.outline,
                    isLoading: state.isScanning,
                    onPressed: state.isScanning
                        ? notifier.stopScan
                        : notifier.startScan,
                  ),
                ),
                if (state.connectedDeviceName != null) ...[
                  SizedBox(width: t.space2),
                  Expanded(
                    child: ErpButton(
                      label: l10n.laserDisconnect,
                      variant: ErpButtonVariant.ghost,
                      onPressed: notifier.disconnect,
                    ),
                  ),
                ],
              ],
            ),
            SizedBox(height: t.space2),
            ErpButton(
              label: l10n.laserSimulateButton,
              variant: ErpButtonVariant.outline,
              onPressed: () =>
                  notifier.simulateReading(l10n.laserSimulatedDeviceName),
            ),
            if (state.devices.isNotEmpty) ...[
              SizedBox(height: t.space3),
              for (final dev in state.devices) ...[
                Padding(
                  padding: EdgeInsets.only(bottom: t.space2),
                  child: ErpCard(
                    children: [
                      Row(
                        children: [
                          Expanded(
                            child: Text(dev.name, style: text.bodyMedium),
                          ),
                          ErpButton(
                            label: l10n.laserConnect,
                            variant: ErpButtonVariant.outline,
                            expand: false,
                            onPressed: () => notifier.connectDevice(dev),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ],
          ],
        ),
      ),
    );
  }
}
