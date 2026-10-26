//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_order_action_request.g.dart';

/// PurchaseOrderActionRequest
///
/// Properties:
/// * [note] 
@BuiltValue()
abstract class PurchaseOrderActionRequest implements Built<PurchaseOrderActionRequest, PurchaseOrderActionRequestBuilder> {
  @BuiltValueField(wireName: r'note')
  String? get note;

  PurchaseOrderActionRequest._();

  factory PurchaseOrderActionRequest([void updates(PurchaseOrderActionRequestBuilder b)]) = _$PurchaseOrderActionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseOrderActionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseOrderActionRequest> get serializer => _$PurchaseOrderActionRequestSerializer();
}

class _$PurchaseOrderActionRequestSerializer implements PrimitiveSerializer<PurchaseOrderActionRequest> {
  @override
  final Iterable<Type> types = const [PurchaseOrderActionRequest, _$PurchaseOrderActionRequest];

  @override
  final String wireName = r'PurchaseOrderActionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseOrderActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    PurchaseOrderActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseOrderActionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  PurchaseOrderActionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseOrderActionRequestBuilder();
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


