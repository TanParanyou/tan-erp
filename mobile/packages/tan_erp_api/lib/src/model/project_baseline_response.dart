//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_baseline_response.g.dart';

/// ProjectBaselineResponse
///
/// Properties:
/// * [quotationId] 
/// * [quotationNumber] 
/// * [contractAmount] 
/// * [quotationSnapshotHash] 
/// * [estimateId] 
/// * [estimateRevisionId] 
/// * [siteSurveyRevisionId] 
/// * [siteSurveySnapshotHash] 
/// * [baselineHash] 
@BuiltValue()
abstract class ProjectBaselineResponse implements Built<ProjectBaselineResponse, ProjectBaselineResponseBuilder> {
  @BuiltValueField(wireName: r'quotationId')
  String? get quotationId;

  @BuiltValueField(wireName: r'quotationNumber')
  String? get quotationNumber;

  @BuiltValueField(wireName: r'contractAmount')
  double? get contractAmount;

  @BuiltValueField(wireName: r'quotationSnapshotHash')
  String? get quotationSnapshotHash;

  @BuiltValueField(wireName: r'estimateId')
  String? get estimateId;

  @BuiltValueField(wireName: r'estimateRevisionId')
  String? get estimateRevisionId;

  @BuiltValueField(wireName: r'siteSurveyRevisionId')
  String? get siteSurveyRevisionId;

  @BuiltValueField(wireName: r'siteSurveySnapshotHash')
  String? get siteSurveySnapshotHash;

  @BuiltValueField(wireName: r'baselineHash')
  String? get baselineHash;

  ProjectBaselineResponse._();

  factory ProjectBaselineResponse([void updates(ProjectBaselineResponseBuilder b)]) = _$ProjectBaselineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBaselineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBaselineResponse> get serializer => _$ProjectBaselineResponseSerializer();
}

class _$ProjectBaselineResponseSerializer implements PrimitiveSerializer<ProjectBaselineResponse> {
  @override
  final Iterable<Type> types = const [ProjectBaselineResponse, _$ProjectBaselineResponse];

  @override
  final String wireName = r'ProjectBaselineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBaselineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.quotationId != null) {
      yield r'quotationId';
      yield serializers.serialize(
        object.quotationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.quotationNumber != null) {
      yield r'quotationNumber';
      yield serializers.serialize(
        object.quotationNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.contractAmount != null) {
      yield r'contractAmount';
      yield serializers.serialize(
        object.contractAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.quotationSnapshotHash != null) {
      yield r'quotationSnapshotHash';
      yield serializers.serialize(
        object.quotationSnapshotHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.estimateId != null) {
      yield r'estimateId';
      yield serializers.serialize(
        object.estimateId,
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
    if (object.siteSurveyRevisionId != null) {
      yield r'siteSurveyRevisionId';
      yield serializers.serialize(
        object.siteSurveyRevisionId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.siteSurveySnapshotHash != null) {
      yield r'siteSurveySnapshotHash';
      yield serializers.serialize(
        object.siteSurveySnapshotHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.baselineHash != null) {
      yield r'baselineHash';
      yield serializers.serialize(
        object.baselineHash,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBaselineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBaselineResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'quotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationId = valueDes;
          break;
        case r'quotationNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationNumber = valueDes;
          break;
        case r'contractAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.contractAmount = valueDes;
          break;
        case r'quotationSnapshotHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationSnapshotHash = valueDes;
          break;
        case r'estimateId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateId = valueDes;
          break;
        case r'estimateRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateRevisionId = valueDes;
          break;
        case r'siteSurveyRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveyRevisionId = valueDes;
          break;
        case r'siteSurveySnapshotHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveySnapshotHash = valueDes;
          break;
        case r'baselineHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.baselineHash = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBaselineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBaselineResponseBuilder();
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


