//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/survey_localized_name_contract.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_survey_design_style_request.g.dart';

/// CreateSurveyDesignStyleRequest
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [description] 
/// * [sortOrder] 
@BuiltValue()
abstract class CreateSurveyDesignStyleRequest implements Built<CreateSurveyDesignStyleRequest, CreateSurveyDesignStyleRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  SurveyLocalizedNameContract? get name;

  @BuiltValueField(wireName: r'description')
  SurveyLocalizedNameContract? get description;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  CreateSurveyDesignStyleRequest._();

  factory CreateSurveyDesignStyleRequest([void updates(CreateSurveyDesignStyleRequestBuilder b)]) = _$CreateSurveyDesignStyleRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateSurveyDesignStyleRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateSurveyDesignStyleRequest> get serializer => _$CreateSurveyDesignStyleRequestSerializer();
}

class _$CreateSurveyDesignStyleRequestSerializer implements PrimitiveSerializer<CreateSurveyDesignStyleRequest> {
  @override
  final Iterable<Type> types = const [CreateSurveyDesignStyleRequest, _$CreateSurveyDesignStyleRequest];

  @override
  final String wireName = r'CreateSurveyDesignStyleRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateSurveyDesignStyleRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
        specifiedType: const FullType(SurveyLocalizedNameContract),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType(SurveyLocalizedNameContract),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateSurveyDesignStyleRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateSurveyDesignStyleRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
            specifiedType: const FullType.nullable(SurveyLocalizedNameContract),
          ) as SurveyLocalizedNameContract?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SurveyLocalizedNameContract),
          ) as SurveyLocalizedNameContract?;
          if (valueDes == null) continue;
          result.description.replace(valueDes);
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateSurveyDesignStyleRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateSurveyDesignStyleRequestBuilder();
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


