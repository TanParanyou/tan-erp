//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/adjustment_line_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'adjust_stock_request.g.dart';

/// AdjustStockRequest
///
/// Properties:
/// * [warehouseId] 
/// * [reason] 
/// * [lines] 
@BuiltValue()
abstract class AdjustStockRequest implements Built<AdjustStockRequest, AdjustStockRequestBuilder> {
  @BuiltValueField(wireName: r'warehouseId')
  String? get warehouseId;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'lines')
  BuiltList<AdjustmentLineRequest>? get lines;

  AdjustStockRequest._();

  factory AdjustStockRequest([void updates(AdjustStockRequestBuilder b)]) = _$AdjustStockRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdjustStockRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdjustStockRequest> get serializer => _$AdjustStockRequestSerializer();
}

class _$AdjustStockRequestSerializer implements PrimitiveSerializer<AdjustStockRequest> {
  @override
  final Iterable<Type> types = const [AdjustStockRequest, _$AdjustStockRequest];

  @override
  final String wireName = r'AdjustStockRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdjustStockRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.warehouseId != null) {
      yield r'warehouseId';
      yield serializers.serialize(
        object.warehouseId,
        specifiedType: const FullType(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdjustmentLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdjustStockRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdjustStockRequestBuilder result,
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
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdjustmentLineRequest)]),
          ) as BuiltList<AdjustmentLineRequest>?;
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
  AdjustStockRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdjustStockRequestBuilder();
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


