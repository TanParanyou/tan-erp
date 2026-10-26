//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/calculation_line_response.dart';
import 'package:tan_erp_api/src/model/qe_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'calculation_response.g.dart';

/// CalculationResponse
///
/// Properties:
/// * [version] 
/// * [templateCode] 
/// * [templateVersion] 
/// * [netAmount] 
/// * [taxAmount] 
/// * [displayedLower] 
/// * [displayedUpper] 
/// * [validUntil] 
/// * [shareDecision] 
/// * [reasonCodes] 
/// * [inputHash] 
/// * [lines] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [branchOverrideId] 
@BuiltValue()
abstract class CalculationResponse implements Built<CalculationResponse, CalculationResponseBuilder> {
  @BuiltValueField(wireName: r'version')
  int? get version;

  @BuiltValueField(wireName: r'templateCode')
  String? get templateCode;

  @BuiltValueField(wireName: r'templateVersion')
  int? get templateVersion;

  @BuiltValueField(wireName: r'netAmount')
  double? get netAmount;

  @BuiltValueField(wireName: r'taxAmount')
  double? get taxAmount;

  @BuiltValueField(wireName: r'displayedLower')
  double? get displayedLower;

  @BuiltValueField(wireName: r'displayedUpper')
  double? get displayedUpper;

  @BuiltValueField(wireName: r'validUntil')
  Date? get validUntil;

  @BuiltValueField(wireName: r'shareDecision')
  String? get shareDecision;

  @BuiltValueField(wireName: r'reasonCodes')
  BuiltList<String>? get reasonCodes;

  @BuiltValueField(wireName: r'inputHash')
  String? get inputHash;

  @BuiltValueField(wireName: r'lines')
  BuiltList<CalculationLineResponse>? get lines;

  @BuiltValueField(wireName: r'createdBy')
  QePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'branchOverrideId')
  String? get branchOverrideId;

  CalculationResponse._();

  factory CalculationResponse([void updates(CalculationResponseBuilder b)]) = _$CalculationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CalculationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CalculationResponse> get serializer => _$CalculationResponseSerializer();
}

class _$CalculationResponseSerializer implements PrimitiveSerializer<CalculationResponse> {
  @override
  final Iterable<Type> types = const [CalculationResponse, _$CalculationResponse];

  @override
  final String wireName = r'CalculationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CalculationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.version != null) {
      yield r'version';
      yield serializers.serialize(
        object.version,
        specifiedType: const FullType(int),
      );
    }
    if (object.templateCode != null) {
      yield r'templateCode';
      yield serializers.serialize(
        object.templateCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.templateVersion != null) {
      yield r'templateVersion';
      yield serializers.serialize(
        object.templateVersion,
        specifiedType: const FullType(int),
      );
    }
    if (object.netAmount != null) {
      yield r'netAmount';
      yield serializers.serialize(
        object.netAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.taxAmount != null) {
      yield r'taxAmount';
      yield serializers.serialize(
        object.taxAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.displayedLower != null) {
      yield r'displayedLower';
      yield serializers.serialize(
        object.displayedLower,
        specifiedType: const FullType(double),
      );
    }
    if (object.displayedUpper != null) {
      yield r'displayedUpper';
      yield serializers.serialize(
        object.displayedUpper,
        specifiedType: const FullType(double),
      );
    }
    if (object.validUntil != null) {
      yield r'validUntil';
      yield serializers.serialize(
        object.validUntil,
        specifiedType: const FullType(Date),
      );
    }
    if (object.shareDecision != null) {
      yield r'shareDecision';
      yield serializers.serialize(
        object.shareDecision,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reasonCodes != null) {
      yield r'reasonCodes';
      yield serializers.serialize(
        object.reasonCodes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.inputHash != null) {
      yield r'inputHash';
      yield serializers.serialize(
        object.inputHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CalculationLineResponse)]),
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
    if (object.branchOverrideId != null) {
      yield r'branchOverrideId';
      yield serializers.serialize(
        object.branchOverrideId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CalculationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CalculationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'version':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.version = valueDes;
          break;
        case r'templateCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.templateCode = valueDes;
          break;
        case r'templateVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.templateVersion = valueDes;
          break;
        case r'netAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.netAmount = valueDes;
          break;
        case r'taxAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxAmount = valueDes;
          break;
        case r'displayedLower':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.displayedLower = valueDes;
          break;
        case r'displayedUpper':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.displayedUpper = valueDes;
          break;
        case r'validUntil':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.validUntil = valueDes;
          break;
        case r'shareDecision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.shareDecision = valueDes;
          break;
        case r'reasonCodes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.reasonCodes.replace(valueDes);
          break;
        case r'inputHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.inputHash = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CalculationLineResponse)]),
          ) as BuiltList<CalculationLineResponse>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
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
        case r'branchOverrideId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchOverrideId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CalculationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CalculationResponseBuilder();
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


