//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'note_request.g.dart';

/// NoteRequest
///
/// Properties:
/// * [note] 
@BuiltValue()
abstract class NoteRequest implements Built<NoteRequest, NoteRequestBuilder> {
  @BuiltValueField(wireName: r'note')
  String? get note;

  NoteRequest._();

  factory NoteRequest([void updates(NoteRequestBuilder b)]) = _$NoteRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(NoteRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<NoteRequest> get serializer => _$NoteRequestSerializer();
}

class _$NoteRequestSerializer implements PrimitiveSerializer<NoteRequest> {
  @override
  final Iterable<Type> types = const [NoteRequest, _$NoteRequest];

  @override
  final String wireName = r'NoteRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    NoteRequest object, {
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
    NoteRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required NoteRequestBuilder result,
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
  NoteRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = NoteRequestBuilder();
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


