//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_value_report_row_response.g.dart';

/// StockValueReportRowResponse
///
/// Properties:
/// * [warehouseId] 
/// * [warehouseCode] 
/// * [warehouseName] 
/// * [branchCode] 
/// * [itemId] 
/// * [itemCode] 
/// * [itemNameTh] 
/// * [itemNameEn] 
/// * [onHand] 
/// * [reserved] 
/// * [available] 
/// * [totalValue] 
@BuiltValue()
abstract class StockValueReportRowResponse implements Built<StockValueReportRowResponse, StockValueReportRowResponseBuilder> {
  @BuiltValueField(wireName: r'warehouseId')
  String? get warehouseId;

  @BuiltValueField(wireName: r'warehouseCode')
  String? get warehouseCode;

  @BuiltValueField(wireName: r'warehouseName')
  String? get warehouseName;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'itemNameTh')
  String? get itemNameTh;

  @BuiltValueField(wireName: r'itemNameEn')
  String? get itemNameEn;

  @BuiltValueField(wireName: r'onHand')
  double? get onHand;

  @BuiltValueField(wireName: r'reserved')
  double? get reserved;

  @BuiltValueField(wireName: r'available')
  double? get available;

  @BuiltValueField(wireName: r'totalValue')
  double? get totalValue;

  StockValueReportRowResponse._();

  factory StockValueReportRowResponse([void updates(StockValueReportRowResponseBuilder b)]) = _$StockValueReportRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockValueReportRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockValueReportRowResponse> get serializer => _$StockValueReportRowResponseSerializer();
}

class _$StockValueReportRowResponseSerializer implements PrimitiveSerializer<StockValueReportRowResponse> {
  @override
  final Iterable<Type> types = const [StockValueReportRowResponse, _$StockValueReportRowResponse];

  @override
  final String wireName = r'StockValueReportRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockValueReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.warehouseId != null) {
      yield r'warehouseId';
      yield serializers.serialize(
        object.warehouseId,
        specifiedType: const FullType(String),
      );
    }
    if (object.warehouseCode != null) {
      yield r'warehouseCode';
      yield serializers.serialize(
        object.warehouseCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.warehouseName != null) {
      yield r'warehouseName';
      yield serializers.serialize(
        object.warehouseName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchCode != null) {
      yield r'branchCode';
      yield serializers.serialize(
        object.branchCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.itemCode != null) {
      yield r'itemCode';
      yield serializers.serialize(
        object.itemCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemNameTh != null) {
      yield r'itemNameTh';
      yield serializers.serialize(
        object.itemNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemNameEn != null) {
      yield r'itemNameEn';
      yield serializers.serialize(
        object.itemNameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.onHand != null) {
      yield r'onHand';
      yield serializers.serialize(
        object.onHand,
        specifiedType: const FullType(double),
      );
    }
    if (object.reserved != null) {
      yield r'reserved';
      yield serializers.serialize(
        object.reserved,
        specifiedType: const FullType(double),
      );
    }
    if (object.available != null) {
      yield r'available';
      yield serializers.serialize(
        object.available,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalValue != null) {
      yield r'totalValue';
      yield serializers.serialize(
        object.totalValue,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockValueReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockValueReportRowResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'warehouseId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.warehouseId = valueDes;
          break;
        case r'warehouseCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.warehouseCode = valueDes;
          break;
        case r'warehouseName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.warehouseName = valueDes;
          break;
        case r'branchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchCode = valueDes;
          break;
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'itemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemCode = valueDes;
          break;
        case r'itemNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemNameTh = valueDes;
          break;
        case r'itemNameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemNameEn = valueDes;
          break;
        case r'onHand':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.onHand = valueDes;
          break;
        case r'reserved':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.reserved = valueDes;
          break;
        case r'available':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.available = valueDes;
          break;
        case r'totalValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalValue = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockValueReportRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockValueReportRowResponseBuilder();
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


