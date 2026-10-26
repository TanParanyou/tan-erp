//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_calculation_snapshot_response.g.dart';

/// EstimateCalculationSnapshotResponse
///
/// Properties:
/// * [id] 
/// * [estimateRevisionId] 
/// * [calculationVersion] 
/// * [inputHash] 
/// * [snapshotJson] 
/// * [calculationPolicyVersionId] 
/// * [taxPolicyVersionId] 
/// * [calculationPolicyVersion] 
/// * [taxPolicyVersion] 
/// * [calculationPolicyHash] 
/// * [taxPolicyHash] 
/// * [capturedByUserId] 
/// * [capturedAtUtc] 
@BuiltValue()
abstract class EstimateCalculationSnapshotResponse implements Built<EstimateCalculationSnapshotResponse, EstimateCalculationSnapshotResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'estimateRevisionId')
  String? get estimateRevisionId;

  @BuiltValueField(wireName: r'calculationVersion')
  int? get calculationVersion;

  @BuiltValueField(wireName: r'inputHash')
  String? get inputHash;

  @BuiltValueField(wireName: r'snapshotJson')
  String? get snapshotJson;

  @BuiltValueField(wireName: r'calculationPolicyVersionId')
  String? get calculationPolicyVersionId;

  @BuiltValueField(wireName: r'taxPolicyVersionId')
  String? get taxPolicyVersionId;

  @BuiltValueField(wireName: r'calculationPolicyVersion')
  String? get calculationPolicyVersion;

  @BuiltValueField(wireName: r'taxPolicyVersion')
  String? get taxPolicyVersion;

  @BuiltValueField(wireName: r'calculationPolicyHash')
  String? get calculationPolicyHash;

  @BuiltValueField(wireName: r'taxPolicyHash')
  String? get taxPolicyHash;

  @BuiltValueField(wireName: r'capturedByUserId')
  String? get capturedByUserId;

  @BuiltValueField(wireName: r'capturedAtUtc')
  DateTime? get capturedAtUtc;

  EstimateCalculationSnapshotResponse._();

  factory EstimateCalculationSnapshotResponse([void updates(EstimateCalculationSnapshotResponseBuilder b)]) = _$EstimateCalculationSnapshotResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateCalculationSnapshotResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateCalculationSnapshotResponse> get serializer => _$EstimateCalculationSnapshotResponseSerializer();
}

class _$EstimateCalculationSnapshotResponseSerializer implements PrimitiveSerializer<EstimateCalculationSnapshotResponse> {
  @override
  final Iterable<Type> types = const [EstimateCalculationSnapshotResponse, _$EstimateCalculationSnapshotResponse];

  @override
  final String wireName = r'EstimateCalculationSnapshotResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateCalculationSnapshotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateRevisionId != null) {
      yield r'estimateRevisionId';
      yield serializers.serialize(
        object.estimateRevisionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.calculationVersion != null) {
      yield r'calculationVersion';
      yield serializers.serialize(
        object.calculationVersion,
        specifiedType: const FullType(int),
      );
    }
    if (object.inputHash != null) {
      yield r'inputHash';
      yield serializers.serialize(
        object.inputHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.snapshotJson != null) {
      yield r'snapshotJson';
      yield serializers.serialize(
        object.snapshotJson,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.calculationPolicyVersionId != null) {
      yield r'calculationPolicyVersionId';
      yield serializers.serialize(
        object.calculationPolicyVersionId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxPolicyVersionId != null) {
      yield r'taxPolicyVersionId';
      yield serializers.serialize(
        object.taxPolicyVersionId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.calculationPolicyVersion != null) {
      yield r'calculationPolicyVersion';
      yield serializers.serialize(
        object.calculationPolicyVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxPolicyVersion != null) {
      yield r'taxPolicyVersion';
      yield serializers.serialize(
        object.taxPolicyVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.calculationPolicyHash != null) {
      yield r'calculationPolicyHash';
      yield serializers.serialize(
        object.calculationPolicyHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxPolicyHash != null) {
      yield r'taxPolicyHash';
      yield serializers.serialize(
        object.taxPolicyHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.capturedByUserId != null) {
      yield r'capturedByUserId';
      yield serializers.serialize(
        object.capturedByUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.capturedAtUtc != null) {
      yield r'capturedAtUtc';
      yield serializers.serialize(
        object.capturedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateCalculationSnapshotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateCalculationSnapshotResponseBuilder result,
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
        case r'estimateRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateRevisionId = valueDes;
          break;
        case r'calculationVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.calculationVersion = valueDes;
          break;
        case r'inputHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.inputHash = valueDes;
          break;
        case r'snapshotJson':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.snapshotJson = valueDes;
          break;
        case r'calculationPolicyVersionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.calculationPolicyVersionId = valueDes;
          break;
        case r'taxPolicyVersionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxPolicyVersionId = valueDes;
          break;
        case r'calculationPolicyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.calculationPolicyVersion = valueDes;
          break;
        case r'taxPolicyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxPolicyVersion = valueDes;
          break;
        case r'calculationPolicyHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.calculationPolicyHash = valueDes;
          break;
        case r'taxPolicyHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxPolicyHash = valueDes;
          break;
        case r'capturedByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.capturedByUserId = valueDes;
          break;
        case r'capturedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.capturedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateCalculationSnapshotResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateCalculationSnapshotResponseBuilder();
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


