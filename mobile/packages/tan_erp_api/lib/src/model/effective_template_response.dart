//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/effective_option_response.dart';
import 'package:tan_erp_api/src/model/effective_add_on_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'effective_template_response.g.dart';

/// EffectiveTemplateResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [version] 
/// * [workType] 
/// * [name] 
/// * [status] 
/// * [measurementRule] 
/// * [unitCode] 
/// * [taxDisplay] 
/// * [validityDays] 
/// * [grades] 
/// * [complexities] 
/// * [addOns] 
/// * [assumptions] 
/// * [exclusions] 
@BuiltValue()
abstract class EffectiveTemplateResponse implements Built<EffectiveTemplateResponse, EffectiveTemplateResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'version')
  int? get version;

  @BuiltValueField(wireName: r'workType')
  String? get workType;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'measurementRule')
  String? get measurementRule;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'taxDisplay')
  String? get taxDisplay;

  @BuiltValueField(wireName: r'validityDays')
  int? get validityDays;

  @BuiltValueField(wireName: r'grades')
  BuiltList<EffectiveOptionResponse>? get grades;

  @BuiltValueField(wireName: r'complexities')
  BuiltList<EffectiveOptionResponse>? get complexities;

  @BuiltValueField(wireName: r'addOns')
  BuiltList<EffectiveAddOnResponse>? get addOns;

  @BuiltValueField(wireName: r'assumptions')
  BuiltList<String>? get assumptions;

  @BuiltValueField(wireName: r'exclusions')
  BuiltList<String>? get exclusions;

  EffectiveTemplateResponse._();

  factory EffectiveTemplateResponse([void updates(EffectiveTemplateResponseBuilder b)]) = _$EffectiveTemplateResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EffectiveTemplateResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EffectiveTemplateResponse> get serializer => _$EffectiveTemplateResponseSerializer();
}

class _$EffectiveTemplateResponseSerializer implements PrimitiveSerializer<EffectiveTemplateResponse> {
  @override
  final Iterable<Type> types = const [EffectiveTemplateResponse, _$EffectiveTemplateResponse];

  @override
  final String wireName = r'EffectiveTemplateResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EffectiveTemplateResponse object, {
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
    if (object.version != null) {
      yield r'version';
      yield serializers.serialize(
        object.version,
        specifiedType: const FullType(int),
      );
    }
    if (object.workType != null) {
      yield r'workType';
      yield serializers.serialize(
        object.workType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.measurementRule != null) {
      yield r'measurementRule';
      yield serializers.serialize(
        object.measurementRule,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxDisplay != null) {
      yield r'taxDisplay';
      yield serializers.serialize(
        object.taxDisplay,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.validityDays != null) {
      yield r'validityDays';
      yield serializers.serialize(
        object.validityDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.grades != null) {
      yield r'grades';
      yield serializers.serialize(
        object.grades,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EffectiveOptionResponse)]),
      );
    }
    if (object.complexities != null) {
      yield r'complexities';
      yield serializers.serialize(
        object.complexities,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EffectiveOptionResponse)]),
      );
    }
    if (object.addOns != null) {
      yield r'addOns';
      yield serializers.serialize(
        object.addOns,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EffectiveAddOnResponse)]),
      );
    }
    if (object.assumptions != null) {
      yield r'assumptions';
      yield serializers.serialize(
        object.assumptions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.exclusions != null) {
      yield r'exclusions';
      yield serializers.serialize(
        object.exclusions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EffectiveTemplateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EffectiveTemplateResponseBuilder result,
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
        case r'version':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.version = valueDes;
          break;
        case r'workType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workType = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'measurementRule':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.measurementRule = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'taxDisplay':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxDisplay = valueDes;
          break;
        case r'validityDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.validityDays = valueDes;
          break;
        case r'grades':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EffectiveOptionResponse)]),
          ) as BuiltList<EffectiveOptionResponse>?;
          if (valueDes == null) continue;
          result.grades.replace(valueDes);
          break;
        case r'complexities':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EffectiveOptionResponse)]),
          ) as BuiltList<EffectiveOptionResponse>?;
          if (valueDes == null) continue;
          result.complexities.replace(valueDes);
          break;
        case r'addOns':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EffectiveAddOnResponse)]),
          ) as BuiltList<EffectiveAddOnResponse>?;
          if (valueDes == null) continue;
          result.addOns.replace(valueDes);
          break;
        case r'assumptions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.assumptions.replace(valueDes);
          break;
        case r'exclusions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.exclusions.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EffectiveTemplateResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EffectiveTemplateResponseBuilder();
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


