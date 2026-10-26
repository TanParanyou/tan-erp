//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_estimate_draft_request.g.dart';

/// CreateEstimateDraftRequest
///
/// Properties:
/// * [opportunityId] 
/// * [siteSurveyRevisionId] 
/// * [currency] 
@BuiltValue()
abstract class CreateEstimateDraftRequest implements Built<CreateEstimateDraftRequest, CreateEstimateDraftRequestBuilder> {
  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'siteSurveyRevisionId')
  String? get siteSurveyRevisionId;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  CreateEstimateDraftRequest._();

  factory CreateEstimateDraftRequest([void updates(CreateEstimateDraftRequestBuilder b)]) = _$CreateEstimateDraftRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateEstimateDraftRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateEstimateDraftRequest> get serializer => _$CreateEstimateDraftRequestSerializer();
}

class _$CreateEstimateDraftRequestSerializer implements PrimitiveSerializer<CreateEstimateDraftRequest> {
  @override
  final Iterable<Type> types = const [CreateEstimateDraftRequest, _$CreateEstimateDraftRequest];

  @override
  final String wireName = r'CreateEstimateDraftRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateEstimateDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.siteSurveyRevisionId != null) {
      yield r'siteSurveyRevisionId';
      yield serializers.serialize(
        object.siteSurveyRevisionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateEstimateDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateEstimateDraftRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'siteSurveyRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveyRevisionId = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateEstimateDraftRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateEstimateDraftRequestBuilder();
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


