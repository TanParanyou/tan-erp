//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_site_survey_request.g.dart';

/// CreateSiteSurveyRequest
///
/// Properties:
/// * [siteId] 
/// * [assignedSurveyorId] 
/// * [scheduledStartUtc] 
/// * [scheduledEndUtc] 
/// * [expectedOpportunityVersion] 
@BuiltValue()
abstract class CreateSiteSurveyRequest implements Built<CreateSiteSurveyRequest, CreateSiteSurveyRequestBuilder> {
  @BuiltValueField(wireName: r'siteId')
  String? get siteId;

  @BuiltValueField(wireName: r'assignedSurveyorId')
  String? get assignedSurveyorId;

  @BuiltValueField(wireName: r'scheduledStartUtc')
  DateTime? get scheduledStartUtc;

  @BuiltValueField(wireName: r'scheduledEndUtc')
  DateTime? get scheduledEndUtc;

  @BuiltValueField(wireName: r'expectedOpportunityVersion')
  String? get expectedOpportunityVersion;

  CreateSiteSurveyRequest._();

  factory CreateSiteSurveyRequest([void updates(CreateSiteSurveyRequestBuilder b)]) = _$CreateSiteSurveyRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateSiteSurveyRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateSiteSurveyRequest> get serializer => _$CreateSiteSurveyRequestSerializer();
}

class _$CreateSiteSurveyRequestSerializer implements PrimitiveSerializer<CreateSiteSurveyRequest> {
  @override
  final Iterable<Type> types = const [CreateSiteSurveyRequest, _$CreateSiteSurveyRequest];

  @override
  final String wireName = r'CreateSiteSurveyRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateSiteSurveyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.siteId != null) {
      yield r'siteId';
      yield serializers.serialize(
        object.siteId,
        specifiedType: const FullType(String),
      );
    }
    if (object.assignedSurveyorId != null) {
      yield r'assignedSurveyorId';
      yield serializers.serialize(
        object.assignedSurveyorId,
        specifiedType: const FullType(String),
      );
    }
    if (object.scheduledStartUtc != null) {
      yield r'scheduledStartUtc';
      yield serializers.serialize(
        object.scheduledStartUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.scheduledEndUtc != null) {
      yield r'scheduledEndUtc';
      yield serializers.serialize(
        object.scheduledEndUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.expectedOpportunityVersion != null) {
      yield r'expectedOpportunityVersion';
      yield serializers.serialize(
        object.expectedOpportunityVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateSiteSurveyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateSiteSurveyRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'siteId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteId = valueDes;
          break;
        case r'assignedSurveyorId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.assignedSurveyorId = valueDes;
          break;
        case r'scheduledStartUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.scheduledStartUtc = valueDes;
          break;
        case r'scheduledEndUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.scheduledEndUtc = valueDes;
          break;
        case r'expectedOpportunityVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedOpportunityVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateSiteSurveyRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateSiteSurveyRequestBuilder();
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


