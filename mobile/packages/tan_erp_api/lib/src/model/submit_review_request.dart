//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'submit_review_request.g.dart';

/// SubmitReviewRequest
///
/// Properties:
/// * [sourceVersion] 
/// * [note] 
@BuiltValue()
abstract class SubmitReviewRequest implements Built<SubmitReviewRequest, SubmitReviewRequestBuilder> {
  @BuiltValueField(wireName: r'sourceVersion')
  int? get sourceVersion;

  @BuiltValueField(wireName: r'note')
  String? get note;

  SubmitReviewRequest._();

  factory SubmitReviewRequest([void updates(SubmitReviewRequestBuilder b)]) = _$SubmitReviewRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SubmitReviewRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SubmitReviewRequest> get serializer => _$SubmitReviewRequestSerializer();
}

class _$SubmitReviewRequestSerializer implements PrimitiveSerializer<SubmitReviewRequest> {
  @override
  final Iterable<Type> types = const [SubmitReviewRequest, _$SubmitReviewRequest];

  @override
  final String wireName = r'SubmitReviewRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SubmitReviewRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sourceVersion != null) {
      yield r'sourceVersion';
      yield serializers.serialize(
        object.sourceVersion,
        specifiedType: const FullType(int),
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
    SubmitReviewRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SubmitReviewRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sourceVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sourceVersion = valueDes;
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
  SubmitReviewRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SubmitReviewRequestBuilder();
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


