//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'checklist_done_request.g.dart';

/// ChecklistDoneRequest
///
/// Properties:
/// * [done] 
@BuiltValue()
abstract class ChecklistDoneRequest implements Built<ChecklistDoneRequest, ChecklistDoneRequestBuilder> {
  @BuiltValueField(wireName: r'done')
  bool? get done;

  ChecklistDoneRequest._();

  factory ChecklistDoneRequest([void updates(ChecklistDoneRequestBuilder b)]) = _$ChecklistDoneRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ChecklistDoneRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ChecklistDoneRequest> get serializer => _$ChecklistDoneRequestSerializer();
}

class _$ChecklistDoneRequestSerializer implements PrimitiveSerializer<ChecklistDoneRequest> {
  @override
  final Iterable<Type> types = const [ChecklistDoneRequest, _$ChecklistDoneRequest];

  @override
  final String wireName = r'ChecklistDoneRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ChecklistDoneRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.done != null) {
      yield r'done';
      yield serializers.serialize(
        object.done,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ChecklistDoneRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ChecklistDoneRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'done':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.done = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ChecklistDoneRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ChecklistDoneRequestBuilder();
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


