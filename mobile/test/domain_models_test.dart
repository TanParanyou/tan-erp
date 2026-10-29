import 'package:flutter_test/flutter_test.dart';
import 'package:tan_erp_field/core/ui/erp_attachment_strip.dart';
import 'package:tan_erp_field/features/notifications/domain/notification_models.dart';
import 'package:tan_erp_field/features/quick_estimate/domain/field_quick_estimate_models.dart';
import 'package:tan_erp_field/features/scan/domain/scan_models.dart';
import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';

void main() {
  group('FieldSurvey models', () {
    test('FieldSurveyMeasurement toJson and fromJson and copyWith', () {
      const measurement = FieldSurveyMeasurement(
        id: 'm-1',
        measurementType: 'area',
        value: 42.5,
        unitCode: 'sqm',
        captureMethod: 'measured',
        notes: 'Living room floor',
      );

      final json = measurement.toJson();
      final restored = FieldSurveyMeasurement.fromJson(json);

      expect(restored.id, equals('m-1'));
      expect(restored.measurementType, equals('area'));
      expect(restored.value, equals(42.5));
      expect(restored.unitCode, equals('sqm'));
      expect(restored.captureMethod, equals('measured'));
      expect(restored.notes, equals('Living room floor'));

      final copied = measurement.copyWith(value: 50);
      expect(copied.value, equals(50));
      expect(copied.id, equals('m-1'));
    });

    test('FieldAttachmentItem toJson and fromJson and copyWith', () {
      const attachment = FieldAttachmentItem(
        id: 'att-1',
        filename: 'room.jpg',
        localPath: '/tmp/room.jpg',
        remoteFileId: 'remote-1',
        isSynced: true,
      );

      final json = attachment.toJson();
      final restored = FieldAttachmentItem.fromJson(json);

      expect(restored.id, equals('att-1'));
      expect(restored.filename, equals('room.jpg'));
      expect(restored.localPath, equals('/tmp/room.jpg'));
      expect(restored.remoteFileId, equals('remote-1'));
      expect(restored.isSynced, isTrue);

      final copied = attachment.copyWith(filename: 'updated.jpg');
      expect(copied.filename, equals('updated.jpg'));
      expect(copied.id, equals('att-1'));
    });

    test('FieldSurveyArea toJson and fromJson and copyWith', () {
      const measurement = FieldSurveyMeasurement(
        id: 'm-1',
        measurementType: 'area',
        value: 20,
        unitCode: 'sqm',
        captureMethod: 'measured',
      );
      const attachment = FieldAttachmentItem(
        id: 'att-1',
        filename: 'area.jpg',
        localPath: '/tmp/area.jpg',
      );
      const area = FieldSurveyArea(
        id: 'area-1',
        code: 'A-01',
        name: 'Master Bedroom',
        description: 'Second floor',
        measurements: [measurement],
        attachments: [attachment],
      );

      final json = area.toJson();
      final restored = FieldSurveyArea.fromJson(json);

      expect(restored.id, equals('area-1'));
      expect(restored.code, equals('A-01'));
      expect(restored.name, equals('Master Bedroom'));
      expect(restored.description, equals('Second floor'));
      expect(restored.measurements.length, equals(1));
      expect(restored.attachments.length, equals(1));

      final copied = area.copyWith(name: 'Guest Bedroom');
      expect(copied.name, equals('Guest Bedroom'));
      expect(copied.id, equals('area-1'));
    });

    test('FieldSurveyRecord toJson and fromJson and copyWith', () {
      final now = DateTime.utc(2026, 10, 29, 10, 30);
      final record = FieldSurveyRecord(
        surveyId: 'srv-1',
        opportunityId: 'opp-1',
        opportunityCode: 'OPP-2026-001',
        opportunityTitle: 'Office Renovation',
        customerName: 'Acme Corp',
        siteName: 'Sathorn Square',
        surveyNumber: 'SRV-2026-001',
        status: 'draft',
        revisionId: 'rev-1',
        revisionNumber: 1,
        rowVersion: '1',
        scopeSummary: 'Floor renovation',
        scheduledAtUtc: now,
      );

      final json = record.toJson();
      final restored = FieldSurveyRecord.fromJson(json);

      expect(restored.surveyId, equals('srv-1'));
      expect(restored.opportunityId, equals('opp-1'));
      expect(restored.opportunityCode, equals('OPP-2026-001'));
      expect(restored.opportunityTitle, equals('Office Renovation'));
      expect(restored.customerName, equals('Acme Corp'));
      expect(restored.siteName, equals('Sathorn Square'));
      expect(restored.surveyNumber, equals('SRV-2026-001'));
      expect(restored.status, equals('draft'));
      expect(restored.revisionId, equals('rev-1'));
      expect(restored.revisionNumber, equals(1));
      expect(restored.rowVersion, equals('1'));
      expect(restored.scopeSummary, equals('Floor renovation'));
      expect(restored.scheduledAtUtc, equals(now));

      final copied = record.copyWith(status: 'ready');
      expect(copied.status, equals('ready'));
      expect(copied.surveyId, equals('srv-1'));
    });
  });

  group('FieldQuickEstimate models', () {
    test('FieldQuickEstimateLine toJson, fromJson, lineTotal, copyWith', () {
      const line = FieldQuickEstimateLine(
        areaCode: 'A-01',
        areaName: 'Lobby',
        quantitySqm: 25,
        unitRate: 1200,
      );

      expect(line.lineTotal, equals(30000));

      final json = line.toJson();
      final restored = FieldQuickEstimateLine.fromJson(json);

      expect(restored.areaCode, equals('A-01'));
      expect(restored.areaName, equals('Lobby'));
      expect(restored.quantitySqm, equals(25));
      expect(restored.unitRate, equals(1200));
      expect(restored.lineTotal, equals(30000));

      final copied = line.copyWith(quantitySqm: 30);
      expect(copied.quantitySqm, equals(30));
      expect(copied.lineTotal, equals(36000));
    });

    test('FieldQuickEstimateRecord serialization, total, copyWith', () {
      const line = FieldQuickEstimateLine(
        areaCode: 'A-01',
        areaName: 'Lobby',
        quantitySqm: 25,
        unitRate: 1200,
      );
      const record = FieldQuickEstimateRecord(
        id: 'qe-1',
        number: 'QE-2026-001',
        opportunityId: 'opp-1',
        surveyRevisionId: 'rev-1',
        propertyType: 'condo',
        gradeCode: 'standard',
        calculationVersion: 1,
        netAmount: 30000,
        taxAmount: 2100,
        displayedLower: 28000,
        displayedUpper: 35000,
        validUntilText: '30 days',
        lines: [line],
        isShared: true,
      );

      expect(record.grossAmount, equals(32100));

      final json = record.toJson();
      final restored = FieldQuickEstimateRecord.fromJson(json);

      expect(restored.id, equals('qe-1'));
      expect(restored.number, equals('QE-2026-001'));
      expect(restored.grossAmount, equals(32100));
      expect(restored.isShared, isTrue);
      expect(restored.isConverted, isFalse);
      expect(restored.lines.length, equals(1));

      final copied = record.copyWith(isConverted: true);
      expect(copied.isConverted, isTrue);
      expect(copied.id, equals('qe-1'));
    });
  });

  group('FieldNotificationItem', () {
    test('FieldNotificationItem toJson, fromJson, isUnread, copyWith', () {
      final now = DateTime.utc(2026, 10, 29, 8);
      final item = FieldNotificationItem(
        id: 'notif-1',
        type: 'estimate_pending_approval',
        payload: const <String, String>{
          'documentNumber': 'EST-202610-0014',
          'title': 'Reception Lobby',
        },
        createdAtUtc: now,
        deepLink: '/estimates/1',
      );

      expect(item.isUnread, isTrue);
      expect(item.displayBody, contains('EST-202610-0014'));

      final json = item.toJson();
      final restored = FieldNotificationItem.fromJson(json);

      expect(restored.id, equals('notif-1'));
      expect(restored.type, equals('estimate_pending_approval'));
      expect(restored.isUnread, isTrue);
      expect(restored.deepLink, equals('/estimates/1'));

      final readItem = item.copyWith(readAtUtc: () => now);
      expect(readItem.isUnread, isFalse);
      expect(readItem.readAtUtc, equals(now));
    });
  });

  group('Scan models', () {
    test('ScannedStockRow toJson and fromJson', () {
      const stock = ScannedStockRow(
        warehouseName: 'Main Warehouse',
        onHand: 100,
        reserved: 20,
        available: 80,
      );

      final json = stock.toJson();
      final restored = ScannedStockRow.fromJson(json);

      expect(restored.warehouseName, equals('Main Warehouse'));
      expect(restored.onHand, equals(100));
      expect(restored.reserved, equals(20));
      expect(restored.available, equals(80));
    });

    test('ScannedItemResult toJson, fromJson, and localizedName', () {
      const stock = ScannedStockRow(
        warehouseName: 'Main Warehouse',
        onHand: 50,
        reserved: 10,
        available: 40,
      );
      final scanNow = DateTime.utc(2026, 10, 29, 9);
      final item = ScannedItemResult(
        itemId: 'item-101',
        code: 'MAT-OAK-01',
        nameTh: 'ไม้โอ๊คแท้',
        nameEn: 'Solid Oak Wood',
        itemType: 'material',
        baseUnitCode: 'sqm',
        baseUnitName: 'Square Meter',
        scannedAtUtc: scanNow,
        barcodeValue: '885000000001',
        packagingLevel: 'box',
        stocks: const [stock],
      );

      expect(item.localizedName('th'), equals('ไม้โอ๊คแท้'));
      expect(item.localizedName('en'), equals('Solid Oak Wood'));

      final json = item.toJson();
      final restored = ScannedItemResult.fromJson(json);

      expect(restored.itemId, equals('item-101'));
      expect(restored.code, equals('MAT-OAK-01'));
      expect(restored.stocks.length, equals(1));
      expect(restored.barcodeValue, equals('885000000001'));
    });
  });
}
