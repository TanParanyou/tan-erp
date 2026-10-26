//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'cost_review_reference_response.g.dart';

/// CostReviewReferenceResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
@BuiltValue()
abstract class CostReviewReferenceResponse implements Built<CostReviewReferenceResponse, CostReviewReferenceResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  LocalizedTextResponse? get name;

  CostReviewReferenceResponse._();

  factory CostReviewReferenceResponse([void updates(CostReviewReferenceResponseBuilder b)]) = _$CostReviewReferenceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CostReviewReferenceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CostReviewReferenceResponse> get serializer => _$CostReviewReferenceResponseSerializer();
}

class _$CostReviewReferenceResponseSerializer implements PrimitiveSerializer<CostReviewReferenceResponse> {
  @override
  final Iterable<Type> types = const [CostReviewReferenceResponse, _$CostReviewReferenceResponse];

  @override
  final String wireName = r'CostReviewReferenceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CostReviewReferenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CostReviewReferenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CostReviewReferenceResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CostReviewReferenceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CostReviewReferenceResponseBuilder();
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


