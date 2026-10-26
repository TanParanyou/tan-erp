//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'review_estimate_request.g.dart';

/// ReviewEstimateRequest
///
/// Properties:
/// * [revisionNo] 
/// * [decision] 
/// * [reasonCode] 
/// * [note] 
@BuiltValue()
abstract class ReviewEstimateRequest implements Built<ReviewEstimateRequest, ReviewEstimateRequestBuilder> {
  @BuiltValueField(wireName: r'revisionNo')
  int? get revisionNo;

  @BuiltValueField(wireName: r'decision')
  String? get decision;

  @BuiltValueField(wireName: r'reasonCode')
  String? get reasonCode;

  @BuiltValueField(wireName: r'note')
  String? get note;

  ReviewEstimateRequest._();

  factory ReviewEstimateRequest([void updates(ReviewEstimateRequestBuilder b)]) = _$ReviewEstimateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReviewEstimateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReviewEstimateRequest> get serializer => _$ReviewEstimateRequestSerializer();
}

class _$ReviewEstimateRequestSerializer implements PrimitiveSerializer<ReviewEstimateRequest> {
  @override
  final Iterable<Type> types = const [ReviewEstimateRequest, _$ReviewEstimateRequest];

  @override
  final String wireName = r'ReviewEstimateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReviewEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.revisionNo != null) {
      yield r'revisionNo';
      yield serializers.serialize(
        object.revisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.decision != null) {
      yield r'decision';
      yield serializers.serialize(
        object.decision,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reasonCode != null) {
      yield r'reasonCode';
      yield serializers.serialize(
        object.reasonCode,
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
    ReviewEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReviewEstimateRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'revisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revisionNo = valueDes;
          break;
        case r'decision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decision = valueDes;
          break;
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonCode = valueDes;
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
  ReviewEstimateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReviewEstimateRequestBuilder();
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


