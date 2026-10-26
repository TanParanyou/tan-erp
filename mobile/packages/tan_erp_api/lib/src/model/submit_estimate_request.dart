//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'submit_estimate_request.g.dart';

/// SubmitEstimateRequest
///
/// Properties:
/// * [revisionNo] 
/// * [calculationVersion] 
/// * [note] 
@BuiltValue()
abstract class SubmitEstimateRequest implements Built<SubmitEstimateRequest, SubmitEstimateRequestBuilder> {
  @BuiltValueField(wireName: r'revisionNo')
  int? get revisionNo;

  @BuiltValueField(wireName: r'calculationVersion')
  int? get calculationVersion;

  @BuiltValueField(wireName: r'note')
  String? get note;

  SubmitEstimateRequest._();

  factory SubmitEstimateRequest([void updates(SubmitEstimateRequestBuilder b)]) = _$SubmitEstimateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SubmitEstimateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SubmitEstimateRequest> get serializer => _$SubmitEstimateRequestSerializer();
}

class _$SubmitEstimateRequestSerializer implements PrimitiveSerializer<SubmitEstimateRequest> {
  @override
  final Iterable<Type> types = const [SubmitEstimateRequest, _$SubmitEstimateRequest];

  @override
  final String wireName = r'SubmitEstimateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SubmitEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.revisionNo != null) {
      yield r'revisionNo';
      yield serializers.serialize(
        object.revisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.calculationVersion != null) {
      yield r'calculationVersion';
      yield serializers.serialize(
        object.calculationVersion,
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
    SubmitEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SubmitEstimateRequestBuilder result,
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
        case r'calculationVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.calculationVersion = valueDes;
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
  SubmitEstimateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SubmitEstimateRequestBuilder();
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


