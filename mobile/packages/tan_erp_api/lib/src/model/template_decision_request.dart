//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'template_decision_request.g.dart';

/// TemplateDecisionRequest
///
/// Properties:
/// * [decision] 
/// * [note] 
@BuiltValue()
abstract class TemplateDecisionRequest implements Built<TemplateDecisionRequest, TemplateDecisionRequestBuilder> {
  @BuiltValueField(wireName: r'decision')
  String? get decision;

  @BuiltValueField(wireName: r'note')
  String? get note;

  TemplateDecisionRequest._();

  factory TemplateDecisionRequest([void updates(TemplateDecisionRequestBuilder b)]) = _$TemplateDecisionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TemplateDecisionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TemplateDecisionRequest> get serializer => _$TemplateDecisionRequestSerializer();
}

class _$TemplateDecisionRequestSerializer implements PrimitiveSerializer<TemplateDecisionRequest> {
  @override
  final Iterable<Type> types = const [TemplateDecisionRequest, _$TemplateDecisionRequest];

  @override
  final String wireName = r'TemplateDecisionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TemplateDecisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.decision != null) {
      yield r'decision';
      yield serializers.serialize(
        object.decision,
        specifiedType: const FullType.nullable(String),
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
    TemplateDecisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TemplateDecisionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'decision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decision = valueDes;
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
  TemplateDecisionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TemplateDecisionRequestBuilder();
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


