//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_period_close_line_response.dart';
import 'package:tan_erp_api/src/model/inventory_person_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'inventory_period_close_response.g.dart';

/// InventoryPeriodCloseResponse
///
/// Properties:
/// * [id] 
/// * [periodYear] 
/// * [periodMonth] 
/// * [periodEndDate] 
/// * [note] 
/// * [lineCount] 
/// * [totalOnHandQuantity] 
/// * [totalValuationAmount] 
/// * [closedBy] 
/// * [closedAtUtc] 
/// * [lines] 
@BuiltValue()
abstract class InventoryPeriodCloseResponse implements Built<InventoryPeriodCloseResponse, InventoryPeriodCloseResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'periodYear')
  int? get periodYear;

  @BuiltValueField(wireName: r'periodMonth')
  int? get periodMonth;

  @BuiltValueField(wireName: r'periodEndDate')
  Date? get periodEndDate;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'lineCount')
  int? get lineCount;

  @BuiltValueField(wireName: r'totalOnHandQuantity')
  double? get totalOnHandQuantity;

  @BuiltValueField(wireName: r'totalValuationAmount')
  double? get totalValuationAmount;

  @BuiltValueField(wireName: r'closedBy')
  InventoryPersonResponse? get closedBy;

  @BuiltValueField(wireName: r'closedAtUtc')
  DateTime? get closedAtUtc;

  @BuiltValueField(wireName: r'lines')
  BuiltList<InventoryPeriodCloseLineResponse>? get lines;

  InventoryPeriodCloseResponse._();

  factory InventoryPeriodCloseResponse([void updates(InventoryPeriodCloseResponseBuilder b)]) = _$InventoryPeriodCloseResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InventoryPeriodCloseResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InventoryPeriodCloseResponse> get serializer => _$InventoryPeriodCloseResponseSerializer();
}

class _$InventoryPeriodCloseResponseSerializer implements PrimitiveSerializer<InventoryPeriodCloseResponse> {
  @override
  final Iterable<Type> types = const [InventoryPeriodCloseResponse, _$InventoryPeriodCloseResponse];

  @override
  final String wireName = r'InventoryPeriodCloseResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InventoryPeriodCloseResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.periodYear != null) {
      yield r'periodYear';
      yield serializers.serialize(
        object.periodYear,
        specifiedType: const FullType(int),
      );
    }
    if (object.periodMonth != null) {
      yield r'periodMonth';
      yield serializers.serialize(
        object.periodMonth,
        specifiedType: const FullType(int),
      );
    }
    if (object.periodEndDate != null) {
      yield r'periodEndDate';
      yield serializers.serialize(
        object.periodEndDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lineCount != null) {
      yield r'lineCount';
      yield serializers.serialize(
        object.lineCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.totalOnHandQuantity != null) {
      yield r'totalOnHandQuantity';
      yield serializers.serialize(
        object.totalOnHandQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalValuationAmount != null) {
      yield r'totalValuationAmount';
      yield serializers.serialize(
        object.totalValuationAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.closedBy != null) {
      yield r'closedBy';
      yield serializers.serialize(
        object.closedBy,
        specifiedType: const FullType(InventoryPersonResponse),
      );
    }
    if (object.closedAtUtc != null) {
      yield r'closedAtUtc';
      yield serializers.serialize(
        object.closedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(InventoryPeriodCloseLineResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    InventoryPeriodCloseResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InventoryPeriodCloseResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'periodYear':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.periodYear = valueDes;
          break;
        case r'periodMonth':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.periodMonth = valueDes;
          break;
        case r'periodEndDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.periodEndDate = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'lineCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.lineCount = valueDes;
          break;
        case r'totalOnHandQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalOnHandQuantity = valueDes;
          break;
        case r'totalValuationAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalValuationAmount = valueDes;
          break;
        case r'closedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryPersonResponse),
          ) as InventoryPersonResponse?;
          if (valueDes == null) continue;
          result.closedBy.replace(valueDes);
          break;
        case r'closedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.closedAtUtc = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(InventoryPeriodCloseLineResponse)]),
          ) as BuiltList<InventoryPeriodCloseLineResponse>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InventoryPeriodCloseResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InventoryPeriodCloseResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


