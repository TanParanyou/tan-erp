//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'receive_goods_receipt_line_request.g.dart';

/// ReceiveGoodsReceiptLineRequest
///
/// Properties:
/// * [itemId] 
/// * [quantity] 
/// * [locationId] 
/// * [lotNumber] 
/// * [serialNumbers] 
@BuiltValue()
abstract class ReceiveGoodsReceiptLineRequest implements Built<ReceiveGoodsReceiptLineRequest, ReceiveGoodsReceiptLineRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'locationId')
  String? get locationId;

  @BuiltValueField(wireName: r'lotNumber')
  String? get lotNumber;

  @BuiltValueField(wireName: r'serialNumbers')
  BuiltList<String>? get serialNumbers;

  ReceiveGoodsReceiptLineRequest._();

  factory ReceiveGoodsReceiptLineRequest([void updates(ReceiveGoodsReceiptLineRequestBuilder b)]) = _$ReceiveGoodsReceiptLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReceiveGoodsReceiptLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReceiveGoodsReceiptLineRequest> get serializer => _$ReceiveGoodsReceiptLineRequestSerializer();
}

class _$ReceiveGoodsReceiptLineRequestSerializer implements PrimitiveSerializer<ReceiveGoodsReceiptLineRequest> {
  @override
  final Iterable<Type> types = const [ReceiveGoodsReceiptLineRequest, _$ReceiveGoodsReceiptLineRequest];

  @override
  final String wireName = r'ReceiveGoodsReceiptLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReceiveGoodsReceiptLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.locationId != null) {
      yield r'locationId';
      yield serializers.serialize(
        object.locationId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lotNumber != null) {
      yield r'lotNumber';
      yield serializers.serialize(
        object.lotNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.serialNumbers != null) {
      yield r'serialNumbers';
      yield serializers.serialize(
        object.serialNumbers,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReceiveGoodsReceiptLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReceiveGoodsReceiptLineRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'locationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.locationId = valueDes;
          break;
        case r'lotNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lotNumber = valueDes;
          break;
        case r'serialNumbers':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.serialNumbers.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReceiveGoodsReceiptLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReceiveGoodsReceiptLineRequestBuilder();
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


