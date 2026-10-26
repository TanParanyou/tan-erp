//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'confirm_request.g.dart';

/// ConfirmRequest
///
/// Properties:
/// * [externalRef] 
/// * [externalAmount] 
@BuiltValue()
abstract class ConfirmRequest implements Built<ConfirmRequest, ConfirmRequestBuilder> {
  @BuiltValueField(wireName: r'externalRef')
  String? get externalRef;

  @BuiltValueField(wireName: r'externalAmount')
  double? get externalAmount;

  ConfirmRequest._();

  factory ConfirmRequest([void updates(ConfirmRequestBuilder b)]) = _$ConfirmRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ConfirmRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ConfirmRequest> get serializer => _$ConfirmRequestSerializer();
}

class _$ConfirmRequestSerializer implements PrimitiveSerializer<ConfirmRequest> {
  @override
  final Iterable<Type> types = const [ConfirmRequest, _$ConfirmRequest];

  @override
  final String wireName = r'ConfirmRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ConfirmRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.externalRef != null) {
      yield r'externalRef';
      yield serializers.serialize(
        object.externalRef,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.externalAmount != null) {
      yield r'externalAmount';
      yield serializers.serialize(
        object.externalAmount,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ConfirmRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ConfirmRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'externalRef':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.externalRef = valueDes;
          break;
        case r'externalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.externalAmount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ConfirmRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ConfirmRequestBuilder();
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


