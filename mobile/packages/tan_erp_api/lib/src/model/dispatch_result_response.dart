//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'dispatch_result_response.g.dart';

/// DispatchResultResponse
///
/// Properties:
/// * [processed] 
/// * [sent] 
/// * [failed] 
/// * [dead] 
@BuiltValue()
abstract class DispatchResultResponse implements Built<DispatchResultResponse, DispatchResultResponseBuilder> {
  @BuiltValueField(wireName: r'processed')
  int? get processed;

  @BuiltValueField(wireName: r'sent')
  int? get sent;

  @BuiltValueField(wireName: r'failed')
  int? get failed;

  @BuiltValueField(wireName: r'dead')
  int? get dead;

  DispatchResultResponse._();

  factory DispatchResultResponse([void updates(DispatchResultResponseBuilder b)]) = _$DispatchResultResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DispatchResultResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DispatchResultResponse> get serializer => _$DispatchResultResponseSerializer();
}

class _$DispatchResultResponseSerializer implements PrimitiveSerializer<DispatchResultResponse> {
  @override
  final Iterable<Type> types = const [DispatchResultResponse, _$DispatchResultResponse];

  @override
  final String wireName = r'DispatchResultResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DispatchResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.processed != null) {
      yield r'processed';
      yield serializers.serialize(
        object.processed,
        specifiedType: const FullType(int),
      );
    }
    if (object.sent != null) {
      yield r'sent';
      yield serializers.serialize(
        object.sent,
        specifiedType: const FullType(int),
      );
    }
    if (object.failed != null) {
      yield r'failed';
      yield serializers.serialize(
        object.failed,
        specifiedType: const FullType(int),
      );
    }
    if (object.dead != null) {
      yield r'dead';
      yield serializers.serialize(
        object.dead,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    DispatchResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DispatchResultResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'processed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.processed = valueDes;
          break;
        case r'sent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sent = valueDes;
          break;
        case r'failed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.failed = valueDes;
          break;
        case r'dead':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.dead = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  DispatchResultResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DispatchResultResponseBuilder();
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


