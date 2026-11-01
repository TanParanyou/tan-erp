//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_return_line_request.g.dart';

/// SupplierReturnLineRequest
///
/// Properties:
/// * [goodsReceiptLineId] 
/// * [quantity] 
@BuiltValue()
abstract class SupplierReturnLineRequest implements Built<SupplierReturnLineRequest, SupplierReturnLineRequestBuilder> {
  @BuiltValueField(wireName: r'goodsReceiptLineId')
  String? get goodsReceiptLineId;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  SupplierReturnLineRequest._();

  factory SupplierReturnLineRequest([void updates(SupplierReturnLineRequestBuilder b)]) = _$SupplierReturnLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierReturnLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierReturnLineRequest> get serializer => _$SupplierReturnLineRequestSerializer();
}

class _$SupplierReturnLineRequestSerializer implements PrimitiveSerializer<SupplierReturnLineRequest> {
  @override
  final Iterable<Type> types = const [SupplierReturnLineRequest, _$SupplierReturnLineRequest];

  @override
  final String wireName = r'SupplierReturnLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierReturnLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.goodsReceiptLineId != null) {
      yield r'goodsReceiptLineId';
      yield serializers.serialize(
        object.goodsReceiptLineId,
        specifiedType: const FullType(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierReturnLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierReturnLineRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'goodsReceiptLineId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.goodsReceiptLineId = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SupplierReturnLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierReturnLineRequestBuilder();
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


