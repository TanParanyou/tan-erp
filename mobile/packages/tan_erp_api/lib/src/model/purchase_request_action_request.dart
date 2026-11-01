//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_request_action_request.g.dart';

/// PurchaseRequestActionRequest
///
/// Properties:
/// * [note] 
@BuiltValue()
abstract class PurchaseRequestActionRequest implements Built<PurchaseRequestActionRequest, PurchaseRequestActionRequestBuilder> {
  @BuiltValueField(wireName: r'note')
  String? get note;

  PurchaseRequestActionRequest._();

  factory PurchaseRequestActionRequest([void updates(PurchaseRequestActionRequestBuilder b)]) = _$PurchaseRequestActionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseRequestActionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseRequestActionRequest> get serializer => _$PurchaseRequestActionRequestSerializer();
}

class _$PurchaseRequestActionRequestSerializer implements PrimitiveSerializer<PurchaseRequestActionRequest> {
  @override
  final Iterable<Type> types = const [PurchaseRequestActionRequest, _$PurchaseRequestActionRequest];

  @override
  final String wireName = r'PurchaseRequestActionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseRequestActionRequest object, {
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
    PurchaseRequestActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseRequestActionRequestBuilder result,
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
  PurchaseRequestActionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseRequestActionRequestBuilder();
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


