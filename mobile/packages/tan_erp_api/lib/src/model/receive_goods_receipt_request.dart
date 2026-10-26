//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'receive_goods_receipt_request.g.dart';

/// ReceiveGoodsReceiptRequest
///
/// Properties:
/// * [goodsReceiptId] 
/// * [warehouseId] 
@BuiltValue()
abstract class ReceiveGoodsReceiptRequest implements Built<ReceiveGoodsReceiptRequest, ReceiveGoodsReceiptRequestBuilder> {
  @BuiltValueField(wireName: r'goodsReceiptId')
  String? get goodsReceiptId;

  @BuiltValueField(wireName: r'warehouseId')
  String? get warehouseId;

  ReceiveGoodsReceiptRequest._();

  factory ReceiveGoodsReceiptRequest([void updates(ReceiveGoodsReceiptRequestBuilder b)]) = _$ReceiveGoodsReceiptRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReceiveGoodsReceiptRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReceiveGoodsReceiptRequest> get serializer => _$ReceiveGoodsReceiptRequestSerializer();
}

class _$ReceiveGoodsReceiptRequestSerializer implements PrimitiveSerializer<ReceiveGoodsReceiptRequest> {
  @override
  final Iterable<Type> types = const [ReceiveGoodsReceiptRequest, _$ReceiveGoodsReceiptRequest];

  @override
  final String wireName = r'ReceiveGoodsReceiptRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReceiveGoodsReceiptRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.goodsReceiptId != null) {
      yield r'goodsReceiptId';
      yield serializers.serialize(
        object.goodsReceiptId,
        specifiedType: const FullType(String),
      );
    }
    if (object.warehouseId != null) {
      yield r'warehouseId';
      yield serializers.serialize(
        object.warehouseId,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReceiveGoodsReceiptRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReceiveGoodsReceiptRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'goodsReceiptId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.goodsReceiptId = valueDes;
          break;
        case r'warehouseId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.warehouseId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReceiveGoodsReceiptRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReceiveGoodsReceiptRequestBuilder();
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


