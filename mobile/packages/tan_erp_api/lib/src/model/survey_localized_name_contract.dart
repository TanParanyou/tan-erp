//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_localized_name_contract.g.dart';

/// SurveyLocalizedNameContract
///
/// Properties:
/// * [thai] 
/// * [english] 
@BuiltValue()
abstract class SurveyLocalizedNameContract implements Built<SurveyLocalizedNameContract, SurveyLocalizedNameContractBuilder> {
  @BuiltValueField(wireName: r'thai')
  String? get thai;

  @BuiltValueField(wireName: r'english')
  String? get english;

  SurveyLocalizedNameContract._();

  factory SurveyLocalizedNameContract([void updates(SurveyLocalizedNameContractBuilder b)]) = _$SurveyLocalizedNameContract;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyLocalizedNameContractBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyLocalizedNameContract> get serializer => _$SurveyLocalizedNameContractSerializer();
}

class _$SurveyLocalizedNameContractSerializer implements PrimitiveSerializer<SurveyLocalizedNameContract> {
  @override
  final Iterable<Type> types = const [SurveyLocalizedNameContract, _$SurveyLocalizedNameContract];

  @override
  final String wireName = r'SurveyLocalizedNameContract';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyLocalizedNameContract object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.thai != null) {
      yield r'thai';
      yield serializers.serialize(
        object.thai,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.english != null) {
      yield r'english';
      yield serializers.serialize(
        object.english,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SurveyLocalizedNameContract object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyLocalizedNameContractBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'thai':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.thai = valueDes;
          break;
        case r'english':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.english = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SurveyLocalizedNameContract deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyLocalizedNameContractBuilder();
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


