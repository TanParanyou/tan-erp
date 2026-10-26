//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/survey_localized_label.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_design_style_option_response.g.dart';

/// SurveyDesignStyleOptionResponse
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [description] 
/// * [sortOrder] 
@BuiltValue()
abstract class SurveyDesignStyleOptionResponse implements Built<SurveyDesignStyleOptionResponse, SurveyDesignStyleOptionResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  SurveyLocalizedLabel? get name;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  SurveyDesignStyleOptionResponse._();

  factory SurveyDesignStyleOptionResponse([void updates(SurveyDesignStyleOptionResponseBuilder b)]) = _$SurveyDesignStyleOptionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyDesignStyleOptionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyDesignStyleOptionResponse> get serializer => _$SurveyDesignStyleOptionResponseSerializer();
}

class _$SurveyDesignStyleOptionResponseSerializer implements PrimitiveSerializer<SurveyDesignStyleOptionResponse> {
  @override
  final Iterable<Type> types = const [SurveyDesignStyleOptionResponse, _$SurveyDesignStyleOptionResponse];

  @override
  final String wireName = r'SurveyDesignStyleOptionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyDesignStyleOptionResponse object, {
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
        specifiedType: const FullType(SurveyLocalizedLabel),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
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
    SurveyDesignStyleOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyDesignStyleOptionResponseBuilder result,
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
            specifiedType: const FullType.nullable(SurveyLocalizedLabel),
          ) as SurveyLocalizedLabel?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
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
  SurveyDesignStyleOptionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyDesignStyleOptionResponseBuilder();
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


