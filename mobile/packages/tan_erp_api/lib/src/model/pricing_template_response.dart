//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/grade_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/complexity_response.dart';
import 'package:tan_erp_api/src/model/qe_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/add_on_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'pricing_template_response.g.dart';

/// PricingTemplateResponse
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
/// * [referenceRate] 
/// * [minimumCharge] 
/// * [baseRangeRate] 
/// * [maxRangeRate] 
/// * [roundingStep] 
/// * [validityDays] 
/// * [taxRate] 
/// * [taxDisplay] 
/// * [directShareLimit] 
/// * [effectiveFrom] 
/// * [effectiveTo] 
/// * [grades] 
/// * [complexities] 
/// * [addOns] 
/// * [lowConfidenceModifier] 
/// * [mediumConfidenceModifier] 
/// * [customMaterialModifier] 
/// * [assumptions] 
/// * [exclusions] 
/// * [createdBy] 
/// * [decidedBy] 
/// * [decisionNote] 
/// * [createdAtUtc] 
/// * [rowVersion] 
@BuiltValue()
abstract class PricingTemplateResponse implements Built<PricingTemplateResponse, PricingTemplateResponseBuilder> {
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

  @BuiltValueField(wireName: r'referenceRate')
  double? get referenceRate;

  @BuiltValueField(wireName: r'minimumCharge')
  double? get minimumCharge;

  @BuiltValueField(wireName: r'baseRangeRate')
  double? get baseRangeRate;

  @BuiltValueField(wireName: r'maxRangeRate')
  double? get maxRangeRate;

  @BuiltValueField(wireName: r'roundingStep')
  double? get roundingStep;

  @BuiltValueField(wireName: r'validityDays')
  int? get validityDays;

  @BuiltValueField(wireName: r'taxRate')
  double? get taxRate;

  @BuiltValueField(wireName: r'taxDisplay')
  String? get taxDisplay;

  @BuiltValueField(wireName: r'directShareLimit')
  double? get directShareLimit;

  @BuiltValueField(wireName: r'effectiveFrom')
  Date? get effectiveFrom;

  @BuiltValueField(wireName: r'effectiveTo')
  Date? get effectiveTo;

  @BuiltValueField(wireName: r'grades')
  BuiltList<GradeResponse>? get grades;

  @BuiltValueField(wireName: r'complexities')
  BuiltList<ComplexityResponse>? get complexities;

  @BuiltValueField(wireName: r'addOns')
  BuiltList<AddOnResponse>? get addOns;

  @BuiltValueField(wireName: r'lowConfidenceModifier')
  double? get lowConfidenceModifier;

  @BuiltValueField(wireName: r'mediumConfidenceModifier')
  double? get mediumConfidenceModifier;

  @BuiltValueField(wireName: r'customMaterialModifier')
  double? get customMaterialModifier;

  @BuiltValueField(wireName: r'assumptions')
  BuiltList<String>? get assumptions;

  @BuiltValueField(wireName: r'exclusions')
  BuiltList<String>? get exclusions;

  @BuiltValueField(wireName: r'createdBy')
  QePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'decidedBy')
  QePersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  PricingTemplateResponse._();

  factory PricingTemplateResponse([void updates(PricingTemplateResponseBuilder b)]) = _$PricingTemplateResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PricingTemplateResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PricingTemplateResponse> get serializer => _$PricingTemplateResponseSerializer();
}

class _$PricingTemplateResponseSerializer implements PrimitiveSerializer<PricingTemplateResponse> {
  @override
  final Iterable<Type> types = const [PricingTemplateResponse, _$PricingTemplateResponse];

  @override
  final String wireName = r'PricingTemplateResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PricingTemplateResponse object, {
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
    if (object.referenceRate != null) {
      yield r'referenceRate';
      yield serializers.serialize(
        object.referenceRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.minimumCharge != null) {
      yield r'minimumCharge';
      yield serializers.serialize(
        object.minimumCharge,
        specifiedType: const FullType(double),
      );
    }
    if (object.baseRangeRate != null) {
      yield r'baseRangeRate';
      yield serializers.serialize(
        object.baseRangeRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.maxRangeRate != null) {
      yield r'maxRangeRate';
      yield serializers.serialize(
        object.maxRangeRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.roundingStep != null) {
      yield r'roundingStep';
      yield serializers.serialize(
        object.roundingStep,
        specifiedType: const FullType(double),
      );
    }
    if (object.validityDays != null) {
      yield r'validityDays';
      yield serializers.serialize(
        object.validityDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.taxRate != null) {
      yield r'taxRate';
      yield serializers.serialize(
        object.taxRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.taxDisplay != null) {
      yield r'taxDisplay';
      yield serializers.serialize(
        object.taxDisplay,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.directShareLimit != null) {
      yield r'directShareLimit';
      yield serializers.serialize(
        object.directShareLimit,
        specifiedType: const FullType(double),
      );
    }
    if (object.effectiveFrom != null) {
      yield r'effectiveFrom';
      yield serializers.serialize(
        object.effectiveFrom,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.effectiveTo != null) {
      yield r'effectiveTo';
      yield serializers.serialize(
        object.effectiveTo,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.grades != null) {
      yield r'grades';
      yield serializers.serialize(
        object.grades,
        specifiedType: const FullType.nullable(BuiltList, [FullType(GradeResponse)]),
      );
    }
    if (object.complexities != null) {
      yield r'complexities';
      yield serializers.serialize(
        object.complexities,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ComplexityResponse)]),
      );
    }
    if (object.addOns != null) {
      yield r'addOns';
      yield serializers.serialize(
        object.addOns,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AddOnResponse)]),
      );
    }
    if (object.lowConfidenceModifier != null) {
      yield r'lowConfidenceModifier';
      yield serializers.serialize(
        object.lowConfidenceModifier,
        specifiedType: const FullType(double),
      );
    }
    if (object.mediumConfidenceModifier != null) {
      yield r'mediumConfidenceModifier';
      yield serializers.serialize(
        object.mediumConfidenceModifier,
        specifiedType: const FullType(double),
      );
    }
    if (object.customMaterialModifier != null) {
      yield r'customMaterialModifier';
      yield serializers.serialize(
        object.customMaterialModifier,
        specifiedType: const FullType(double),
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
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(QePersonResponse),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(QePersonResponse),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PricingTemplateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PricingTemplateResponseBuilder result,
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
        case r'referenceRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.referenceRate = valueDes;
          break;
        case r'minimumCharge':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.minimumCharge = valueDes;
          break;
        case r'baseRangeRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.baseRangeRate = valueDes;
          break;
        case r'maxRangeRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.maxRangeRate = valueDes;
          break;
        case r'roundingStep':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.roundingStep = valueDes;
          break;
        case r'validityDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.validityDays = valueDes;
          break;
        case r'taxRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxRate = valueDes;
          break;
        case r'taxDisplay':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxDisplay = valueDes;
          break;
        case r'directShareLimit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.directShareLimit = valueDes;
          break;
        case r'effectiveFrom':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveFrom = valueDes;
          break;
        case r'effectiveTo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveTo = valueDes;
          break;
        case r'grades':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(GradeResponse)]),
          ) as BuiltList<GradeResponse>?;
          if (valueDes == null) continue;
          result.grades.replace(valueDes);
          break;
        case r'complexities':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ComplexityResponse)]),
          ) as BuiltList<ComplexityResponse>?;
          if (valueDes == null) continue;
          result.complexities.replace(valueDes);
          break;
        case r'addOns':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AddOnResponse)]),
          ) as BuiltList<AddOnResponse>?;
          if (valueDes == null) continue;
          result.addOns.replace(valueDes);
          break;
        case r'lowConfidenceModifier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.lowConfidenceModifier = valueDes;
          break;
        case r'mediumConfidenceModifier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.mediumConfidenceModifier = valueDes;
          break;
        case r'customMaterialModifier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.customMaterialModifier = valueDes;
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
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QePersonResponse),
          ) as QePersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QePersonResponse),
          ) as QePersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PricingTemplateResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PricingTemplateResponseBuilder();
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


