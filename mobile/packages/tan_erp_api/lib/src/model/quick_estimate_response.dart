//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/calculation_response.dart';
import 'package:tan_erp_api/src/model/conversion_response.dart';
import 'package:tan_erp_api/src/model/qe_person_response.dart';
import 'package:tan_erp_api/src/model/measurement_response.dart';
import 'package:tan_erp_api/src/model/quick_estimate_template_ref_response.dart';
import 'package:tan_erp_api/src/model/share_response.dart';
import 'package:tan_erp_api/src/model/review_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quick_estimate_response.g.dart';

/// QuickEstimateResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [status] 
/// * [customerId] 
/// * [opportunityId] 
/// * [template] 
/// * [propertyType] 
/// * [roomOrArea] 
/// * [gradeCode] 
/// * [complexityCodes] 
/// * [addOnCodes] 
/// * [measurementConfidence] 
/// * [customMaterial] 
/// * [measurements] 
/// * [currentCalculationVersion] 
/// * [currentCalculation] 
/// * [reviews] 
/// * [shares] 
/// * [conversions] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [updatedAtUtc] 
/// * [rowVersion] 
@BuiltValue()
abstract class QuickEstimateResponse implements Built<QuickEstimateResponse, QuickEstimateResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'customerId')
  String? get customerId;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'template')
  QuickEstimateTemplateRefResponse? get template;

  @BuiltValueField(wireName: r'propertyType')
  String? get propertyType;

  @BuiltValueField(wireName: r'roomOrArea')
  String? get roomOrArea;

  @BuiltValueField(wireName: r'gradeCode')
  String? get gradeCode;

  @BuiltValueField(wireName: r'complexityCodes')
  BuiltList<String>? get complexityCodes;

  @BuiltValueField(wireName: r'addOnCodes')
  BuiltList<String>? get addOnCodes;

  @BuiltValueField(wireName: r'measurementConfidence')
  String? get measurementConfidence;

  @BuiltValueField(wireName: r'customMaterial')
  bool? get customMaterial;

  @BuiltValueField(wireName: r'measurements')
  BuiltList<MeasurementResponse>? get measurements;

  @BuiltValueField(wireName: r'currentCalculationVersion')
  int? get currentCalculationVersion;

  @BuiltValueField(wireName: r'currentCalculation')
  CalculationResponse? get currentCalculation;

  @BuiltValueField(wireName: r'reviews')
  BuiltList<ReviewResponse>? get reviews;

  @BuiltValueField(wireName: r'shares')
  BuiltList<ShareResponse>? get shares;

  @BuiltValueField(wireName: r'conversions')
  BuiltList<ConversionResponse>? get conversions;

  @BuiltValueField(wireName: r'createdBy')
  QePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  QuickEstimateResponse._();

  factory QuickEstimateResponse([void updates(QuickEstimateResponseBuilder b)]) = _$QuickEstimateResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuickEstimateResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuickEstimateResponse> get serializer => _$QuickEstimateResponseSerializer();
}

class _$QuickEstimateResponseSerializer implements PrimitiveSerializer<QuickEstimateResponse> {
  @override
  final Iterable<Type> types = const [QuickEstimateResponse, _$QuickEstimateResponse];

  @override
  final String wireName = r'QuickEstimateResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuickEstimateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
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
    if (object.customerId != null) {
      yield r'customerId';
      yield serializers.serialize(
        object.customerId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.template != null) {
      yield r'template';
      yield serializers.serialize(
        object.template,
        specifiedType: const FullType(QuickEstimateTemplateRefResponse),
      );
    }
    if (object.propertyType != null) {
      yield r'propertyType';
      yield serializers.serialize(
        object.propertyType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.roomOrArea != null) {
      yield r'roomOrArea';
      yield serializers.serialize(
        object.roomOrArea,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.gradeCode != null) {
      yield r'gradeCode';
      yield serializers.serialize(
        object.gradeCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.complexityCodes != null) {
      yield r'complexityCodes';
      yield serializers.serialize(
        object.complexityCodes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.addOnCodes != null) {
      yield r'addOnCodes';
      yield serializers.serialize(
        object.addOnCodes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.measurementConfidence != null) {
      yield r'measurementConfidence';
      yield serializers.serialize(
        object.measurementConfidence,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.customMaterial != null) {
      yield r'customMaterial';
      yield serializers.serialize(
        object.customMaterial,
        specifiedType: const FullType(bool),
      );
    }
    if (object.measurements != null) {
      yield r'measurements';
      yield serializers.serialize(
        object.measurements,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MeasurementResponse)]),
      );
    }
    if (object.currentCalculationVersion != null) {
      yield r'currentCalculationVersion';
      yield serializers.serialize(
        object.currentCalculationVersion,
        specifiedType: const FullType(int),
      );
    }
    if (object.currentCalculation != null) {
      yield r'currentCalculation';
      yield serializers.serialize(
        object.currentCalculation,
        specifiedType: const FullType(CalculationResponse),
      );
    }
    if (object.reviews != null) {
      yield r'reviews';
      yield serializers.serialize(
        object.reviews,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ReviewResponse)]),
      );
    }
    if (object.shares != null) {
      yield r'shares';
      yield serializers.serialize(
        object.shares,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ShareResponse)]),
      );
    }
    if (object.conversions != null) {
      yield r'conversions';
      yield serializers.serialize(
        object.conversions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ConversionResponse)]),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(QePersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
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
    QuickEstimateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuickEstimateResponseBuilder result,
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'customerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerId = valueDes;
          break;
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'template':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuickEstimateTemplateRefResponse),
          ) as QuickEstimateTemplateRefResponse?;
          if (valueDes == null) continue;
          result.template.replace(valueDes);
          break;
        case r'propertyType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.propertyType = valueDes;
          break;
        case r'roomOrArea':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roomOrArea = valueDes;
          break;
        case r'gradeCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.gradeCode = valueDes;
          break;
        case r'complexityCodes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.complexityCodes.replace(valueDes);
          break;
        case r'addOnCodes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.addOnCodes.replace(valueDes);
          break;
        case r'measurementConfidence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.measurementConfidence = valueDes;
          break;
        case r'customMaterial':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.customMaterial = valueDes;
          break;
        case r'measurements':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MeasurementResponse)]),
          ) as BuiltList<MeasurementResponse>?;
          if (valueDes == null) continue;
          result.measurements.replace(valueDes);
          break;
        case r'currentCalculationVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.currentCalculationVersion = valueDes;
          break;
        case r'currentCalculation':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CalculationResponse),
          ) as CalculationResponse?;
          if (valueDes == null) continue;
          result.currentCalculation.replace(valueDes);
          break;
        case r'reviews':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ReviewResponse)]),
          ) as BuiltList<ReviewResponse>?;
          if (valueDes == null) continue;
          result.reviews.replace(valueDes);
          break;
        case r'shares':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ShareResponse)]),
          ) as BuiltList<ShareResponse>?;
          if (valueDes == null) continue;
          result.shares.replace(valueDes);
          break;
        case r'conversions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ConversionResponse)]),
          ) as BuiltList<ConversionResponse>?;
          if (valueDes == null) continue;
          result.conversions.replace(valueDes);
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QePersonResponse),
          ) as QePersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
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
  QuickEstimateResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuickEstimateResponseBuilder();
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


