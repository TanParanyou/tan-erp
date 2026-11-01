//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_request_line_request.g.dart';

/// PurchaseRequestLineRequest
///
/// Properties:
/// * [itemId] 
/// * [quantity] 
/// * [estimatedUnitPrice] 
/// * [note] 
@BuiltValue()
abstract class PurchaseRequestLineRequest implements Built<PurchaseRequestLineRequest, PurchaseRequestLineRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'estimatedUnitPrice')
  double? get estimatedUnitPrice;

  @BuiltValueField(wireName: r'note')
  String? get note;

  PurchaseRequestLineRequest._();

  factory PurchaseRequestLineRequest([void updates(PurchaseRequestLineRequestBuilder b)]) = _$PurchaseRequestLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseRequestLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseRequestLineRequest> get serializer => _$PurchaseRequestLineRequestSerializer();
}

class _$PurchaseRequestLineRequestSerializer implements PrimitiveSerializer<PurchaseRequestLineRequest> {
  @override
  final Iterable<Type> types = const [PurchaseRequestLineRequest, _$PurchaseRequestLineRequest];

  @override
  final String wireName = r'PurchaseRequestLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseRequestLineRequest object, {
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
        specifiedType: const FullType(double),
      );
    }
    if (object.estimatedUnitPrice != null) {
      yield r'estimatedUnitPrice';
      yield serializers.serialize(
        object.estimatedUnitPrice,
        specifiedType: const FullType(double),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseRequestLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseRequestLineRequestBuilder result,
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
        case r'estimatedUnitPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.estimatedUnitPrice = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseRequestLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseRequestLineRequestBuilder();
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


